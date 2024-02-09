using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [SerializeField] private WeaponElement _weaponElementPrefab;
    [SerializeField] private Color _color = Color.green;

    private readonly Dictionary<string, WeaponElement> _weaponElements = new Dictionary<string, WeaponElement>();

    public void AddElement(string weaponId, Sprite icon, int clipAmmo, int maxAmmo)
    {
        // Check if the element already exists to update it
        if (_weaponElements.TryGetValue(weaponId, out WeaponElement existingElement))
        {
            existingElement.SetIcon(icon);
            existingElement.SetAmmoText(clipAmmo, maxAmmo);
        }
        else
        {
            // Instantiate a new weapon element
            WeaponElement newElement = Instantiate(_weaponElementPrefab, transform);
            newElement.SetIcon(icon);
            newElement.SetAmmoText(clipAmmo, maxAmmo);
            _weaponElements.Add(weaponId, newElement);
        }
    }

    public void UpdateElementAmmo(string weaponId, int clipAmmo, int maxAmmo)
    {
        if (_weaponElements.TryGetValue(weaponId, out WeaponElement element))
        {
            element.SetAmmoText(clipAmmo, maxAmmo);
        }
    }

    public void RemoveElement(string weaponId)
    {
        if (!_weaponElements.TryGetValue(weaponId, out WeaponElement element)) return;
        Destroy(element.gameObject);
        _weaponElements.Remove(weaponId);
    }
    
    public void SetActive(string weaponID, bool isActive)
    {
        if (!_weaponElements.TryGetValue(weaponID, out WeaponElement element)) return;
        Color inactiveColor = new Color(_color.r, _color.g, _color.b, 0.3f);
        element.SetColor(isActive ? _color : inactiveColor);
    }
    
    
}