using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AetherInputModule : BaseInputModule {

    private float m_NextAction;

    protected AetherInputModule() { }

    [SerializeField]
    private float m_InputActionsPerSecond = 10;

    [SerializeField]
    private bool m_ForceModuleActive;

    public bool forceModuleActive {
        get { return m_ForceModuleActive; }
        set { m_ForceModuleActive = value; }
    }

    public void Start() {
    }


    public override bool IsModuleSupported()
    {
        return true;
    }

    public override void UpdateModule() {
    }

    public override void ActivateModule() {
        base.ActivateModule();

        var toSelect = eventSystem.currentSelectedGameObject;
        if (toSelect == null)
            toSelect = eventSystem.lastSelectedGameObject;
        if (toSelect == null)
            toSelect = eventSystem.firstSelectedGameObject;

        Debug.Log("Aether Input Activated");

        eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
    }

    public override bool ShouldActivateModule() {

        if (!base.ShouldActivateModule())
            return false;

        Vector2 joystick = HololensInput.GetJoystick();

        var shouldActivate = HololensInput.GetPressed(ButtonType.AButton);
        shouldActivate |= HololensInput.GetPressed(ButtonType.BButton);
        shouldActivate |= !Mathf.Approximately(joystick.x, 0.0f);
        shouldActivate |= !Mathf.Approximately(joystick.y, 0.0f);
        return shouldActivate;
    }

    public override void DeactivateModule()
    {
        base.DeactivateModule();
        ClearSelection();
        // Hide selection??
    }

    public override void Process()
    {
        bool usedEvent = SendUpdateEventToSelectedObject();

        if (eventSystem.sendNavigationEvents)
        {
            if (!usedEvent)
                usedEvent |= SendMoveEventToSelectedObject();

            if (!usedEvent)
                SendSubmitEventToSelectedObject();
        }
    }

    private bool SendSubmitEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return false;

        var data = GetBaseEventData();
        if (HololensInput.GetPressed(ButtonType.AButton)) {
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);
        }

        if (HololensInput.GetPressed(ButtonType.BButton))
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
        return data.used;
    }

    private bool AllowMoveEventProcessing(float time) {
        Vector2 joystick = HololensInput.GetJoystick();

        bool allow = !Mathf.Approximately(joystick.x, 0.0f);
        allow |= !Mathf.Approximately(joystick.y, 0.0f);
        allow |= (time > m_NextAction);
        return allow;
    }

    private bool SendMoveEventToSelectedObject() {
        float time = Time.unscaledTime;

        if (!AllowMoveEventProcessing(time))
            return false;

        Vector2 movement = HololensInput.GetJoystick();
        // Debug.Log(m_ProcessingEvent.rawType + " axis:" + m_AllowAxisEvents + " value:" + "(" + x + "," + y + ")");
        var axisEventData = GetAxisEventData(movement.x, movement.y, 0.6f);
        if (!Mathf.Approximately(axisEventData.moveVector.x, 0f)
            || !Mathf.Approximately(axisEventData.moveVector.y, 0f)) {
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
        }
        m_NextAction = time + 1f / m_InputActionsPerSecond;
        return axisEventData.used;
    }

    private bool SendUpdateEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return false;

        var data = GetBaseEventData();
        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
        return data.used;
    }

    protected void ClearSelection() {
        var baseEventData = GetBaseEventData();

        /*foreach (var pointer in m_PointerData.Values)
        {
            // clear all selection
            HandlePointerExitAndEnter(pointer, null);
        }

        m_PointerData.Clear();*/
        eventSystem.SetSelectedGameObject(null, baseEventData);
    }

}