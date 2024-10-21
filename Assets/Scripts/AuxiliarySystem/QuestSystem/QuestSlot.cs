using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuestSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Text titleText;
    private Outline outline;
    private Button button;

    private void Awake() {
        button = GetComponentInChildren<Button>();
        outline = GetComponent<Outline>();

        outline.effectColor = Color.clear;
        outline.effectDistance = Vector2.zero;
    }

    public void Select() {
        outline.effectColor = Color.yellow; // ѡ��ʱ���
        outline.effectDistance = new Vector2(3f, 3f);
    }

    public void Deselect() {
        outline.effectColor = Color.clear; // ȡ��ѡ��ʱ�����ɫ
        outline.effectDistance = Vector2.zero;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (outline.effectColor != Color.yellow) {
            outline.effectColor = Color.white;
            outline.effectDistance = new Vector2(2f, 2f);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (outline.effectColor != Color.yellow) {
            outline.effectColor = Color.clear;
            outline.effectDistance = Vector2.zero;
        }
    }
}