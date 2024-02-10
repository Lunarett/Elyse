using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponElement : MonoBehaviour
{
    [SerializeField] private Image _weaponIcon;
    [SerializeField] private TMP_Text _weaponAmmoText;

    public void SetIcon(Sprite icon)
    {
        _weaponIcon.sprite = icon;
    }

    public void SetAmmoText(int clipAmmo, int maxAmmo)
    {
        _weaponAmmoText.text = $"{clipAmmo}/{maxAmmo}";
    }

    public void SetColor(Color color)
    {
        _weaponAmmoText.color = color;
        _weaponIcon.color = color;
    }
}
