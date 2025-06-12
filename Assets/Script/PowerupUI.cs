using UnityEngine;
using TMPro;

public class PowerupUI : MonoBehaviour
{
    public TextMeshProUGUI jetpackText;
    public TextMeshProUGUI explosionText;
    public TextMeshProUGUI wallExplosionText;

    private int jetpackCount = 0;
    private int explosionCount = 0;
    private int wallExplosionCount = 0;

    public void AddJetpack()
    {
        jetpackCount++;
        jetpackText.text = "x" + jetpackCount;
    }

    public void AddExplosion()
    {
        explosionCount++;
        explosionText.text = "x" + explosionCount;
    }

    public void AddWallExplosion()
    {
        wallExplosionCount++;
        wallExplosionText.text = "x" + wallExplosionCount;
    }

    public void RemoveJetpack()
    {
        jetpackCount = Mathf.Max(0, jetpackCount - 1);
        jetpackText.text = "x" + jetpackCount;
    }

    public void RemoveExplosion()
    {
        explosionCount = Mathf.Max(0, explosionCount - 1);
        explosionText.text = "x" + explosionCount;
    }

    public void RemoveWallExplosion()
    {
        wallExplosionCount = Mathf.Max(0, wallExplosionCount - 1);
        wallExplosionText.text = "x" + wallExplosionCount;
    }
}