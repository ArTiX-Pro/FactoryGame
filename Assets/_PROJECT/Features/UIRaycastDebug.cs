using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIRaycastDebug : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text output;
    private readonly List<RaycastResult> results = new();

    void Update()
    {
        EventSystem eventSystem = EventSystem.current;
        PointerEventData data = new PointerEventData(eventSystem) { position = Mouse.current.position.ReadValue() };
        results.Clear();
        eventSystem.RaycastAll(data, results);

        output.text = $"EventSystems: {FindObjectsByType<EventSystem>(FindObjectsSortMode.None).Length}\n" +
                      $"Lock: {Cursor.lockState}\nMouse: {data.position}\n" +
                      (results.Count > 0 ? "Top hit: " + results[0].gameObject.name : "No hit");
    }
}