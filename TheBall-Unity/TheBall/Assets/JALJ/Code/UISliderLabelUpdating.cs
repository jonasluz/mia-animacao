using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UISliderLabelUpdating : MonoBehaviour {

    public Slider slider;

    Text label;
    string labelText;

    void Awake()
    {
        label = GetComponent<Text>();
        labelText = label.text;
    }
	
	public void LateUpdate()
    {
        label.text = labelText + " " + slider.value + " ";
	}
}
