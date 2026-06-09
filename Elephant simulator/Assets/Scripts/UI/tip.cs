using TMPro;
using UnityEngine;

public class tip : MonoBehaviour
{
    [SerializeField] private TMP_Text tipText; // The text to display in the tip
    
    void Start()
    {
        tipGet();
    }
    private void tipGet()
    {
        int randomTip = Random.Range(0, 5); // Assuming you have 5 tips
        switch (randomTip)
        {
            case 0:
                tipText.text = "Tip: Noise from pushing trees or performing a Power Attack may attract Kumki.";
                break;
            case 1:
                tipText.text = "Tip: The higher your anxiety, the faster your energy drains.";
                break;
            case 2:
                tipText.text = "Tip: Use buildings to play hide and seek with Kumki!";
                break;
            case 3:
                tipText.text = "Tip: Avoid or attack the vehicles to prevent your mental health";
                break;
            case 4:
                tipText.text = "Tip: Poor young elephant doesn't dare to face Kumki!";
                break;
        }
    }

}
