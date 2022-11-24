using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;

public class DropdownScript : MonoBehaviour, IPointerClickHandler {
    public ARDebug DebugScript;
    private RectTransform container;
    private bool isOpen;
    // Use this for initialization

    private ValueAnim<Vector2, Vector2Animable> anim;

    void Start()
    {
        container = transform.Find("Container").GetComponent<RectTransform>();

        Vector3 scale = container.localScale;
        scale.y = 0.0f;
        container.localScale = scale;
        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (anim != null)
        {
            container.localScale = anim.Update(Time.deltaTime);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        string NameofObject = gameObject.name;
        isOpen = !isOpen;

        Vector2 end = container.localScale;
        end.y = isOpen ? 1 : 0;
        anim = new ValueAnim<Vector2, Vector2Animable>(container.localScale, end, 0.4f, Easing.easeInElastic);
        DebugScript.Log(NameofObject+" was pressed");
    }
}