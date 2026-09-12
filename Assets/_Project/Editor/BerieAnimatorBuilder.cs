using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

// Builds the BeriePlayer controller with the Unity API itself (no hand YAML)
// and wires it to Berie Visual. Runs automatically on editor load/compile and
// does nothing once the controller is complete.
[InitializeOnLoad]
public static class BerieAnimatorBuilder
{
    private const string ControllerPath = "Assets/_Project/Animation/BeriePlayer.controller";
    private const string ScenePath = "Assets/_Project/Scenes/MovementLab.unity";
    private const string ClipDir = "Assets/_Project/Animation/";

    static BerieAnimatorBuilder()
    {
        EditorApplication.delayCall += BuildOnce;
    }

    private static void BuildOnce()
    {
        var existing = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (existing != null && existing.layers.Length > 0 && existing.layers[0].stateMachine.states.Length == 4)
        {
            return;
        }

        var idle = LoadClip("BerieIdle.anim");
        var run = LoadClip("BerieRun.anim");
        var jump = LoadClip("BerieJump.anim");
        var fall = LoadClip("BerieFall.anim");
        if (idle == null || run == null || jump == null || fall == null)
        {
            return;
        }

        var ctrl = existing;
        if (ctrl == null)
        {
            ctrl = new AnimatorController();
            AssetDatabase.CreateAsset(ctrl, ControllerPath);
        }

        while (ctrl.layers.Length > 0)
        {
            ctrl.RemoveLayer(0);
        }

        foreach (var name in new[] { "Speed", "Grounded", "VelocityY" })
        {
            if (!HasParameter(ctrl, name))
            {
                var type = name == "Grounded"
                    ? AnimatorControllerParameterType.Bool
                    : AnimatorControllerParameterType.Float;
                ctrl.AddParameter(name, type);
            }
        }

        ctrl.AddLayer("Base Layer");
        var machine = ctrl.layers[0].stateMachine;

        var idleState = machine.AddState("Idle");
        idleState.motion = idle;
        var runState = machine.AddState("Run");
        runState.motion = run;
        var jumpState = machine.AddState("Jump");
        jumpState.motion = jump;
        var fallState = machine.AddState("Fall");
        fallState.motion = fall;
        machine.defaultState = idleState;

        AddTransition(idleState, runState, AnimatorConditionMode.Greater, 0.1f, "Speed", 0.12f);
        AddTransition(runState, idleState, AnimatorConditionMode.Less, 0.1f, "Speed", 0.12f);
        AddTransition(idleState, jumpState, AnimatorConditionMode.IfNot, 0f, "Grounded", 0.05f);
        AddTransition(runState, jumpState, AnimatorConditionMode.IfNot, 0f, "Grounded", 0.05f);
        AddTransition(jumpState, fallState, AnimatorConditionMode.Less, -0.5f, "VelocityY", 0.05f);
        AddTransition(fallState, runState, AnimatorConditionMode.Greater, 0.1f, "Speed", 0.05f, true);
        AddTransition(fallState, idleState, AnimatorConditionMode.Less, 0.1f, "Speed", 0.05f, true);
        AddTransition(jumpState, runState, AnimatorConditionMode.Greater, 0.1f, "Speed", 0.05f, true);
        AddTransition(jumpState, idleState, AnimatorConditionMode.Less, 0.1f, "Speed", 0.05f, true);

        AssetDatabase.SaveAssets();

        var scene = EditorSceneManager.GetActiveScene();
        var visual = GameObject.Find("Berie Visual");
        if (visual == null && scene.path != ScenePath)
        {
            scene = EditorSceneManager.OpenScene(ScenePath);
            visual = GameObject.Find("Berie Visual");
        }

        if (visual == null)
        {
            Debug.LogError("BerieAnimatorBuilder: 'Berie Visual' not found.", ctrl);
            return;
        }

        var animator = visual.GetComponent<Animator>();
        if (animator == null)
        {
            animator = visual.AddComponent<Animator>();
        }

        animator.runtimeAnimatorController = ctrl;
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("BerieAnimatorBuilder: controller built with 4 states and wired to Berie Visual.", ctrl);
    }

    private static AnimationClip LoadClip(string file)
    {
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(ClipDir + file);
        if (clip == null)
        {
            Debug.LogError("BerieAnimatorBuilder: clip missing: " + ClipDir + file);
        }

        return clip;
    }

    private static bool HasParameter(AnimatorController ctrl, string name)
    {
        foreach (var p in ctrl.parameters)
        {
            if (p.name == name)
            {
                return true;
            }
        }

        return false;
    }

    private static void AddTransition(AnimatorState from, AnimatorState to, AnimatorConditionMode mode, float threshold, string parameter, float duration, bool requireGrounded = false)
    {
        var transition = from.AddTransition(to);
        transition.hasExitTime = false;
        transition.duration = duration;
        if (requireGrounded)
        {
            transition.AddCondition(AnimatorConditionMode.If, 0f, "Grounded");
        }

        transition.AddCondition(mode, threshold, parameter);
    }
}
