using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GymController))]
public class GymEditor : Editor
{
    private string[] defaultNames = new string[]
    {
        "Iron Ivan",
        "Mad Mike",
        "Bullet Bruno",
        "Silent Sam",
        "Big Bear Boris",
        "Lightning Leo",
        "Crusher Carl",
        "Storm Sergei"
    };

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GymController controller = (GymController)target;

        if (GUILayout.Button("Заполнить gyms бойцами"))
        {
            controller.gyms.Clear();

            for (int i = 0; i < defaultNames.Length; i++)
            {
                Gym newGym = new Gym
                {
                    name = defaultNames[i],
                    level = 1,
                    income = 0,
                    isAvialiable = true
                };

                newGym.UpdateUpgradeCost();
                controller.gyms.Add(newGym);
            }

            EditorUtility.SetDirty(controller);
        }
    }
}