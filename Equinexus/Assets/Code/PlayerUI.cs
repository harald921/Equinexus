using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] GameObject _hand;

    [Space(10)]
    [SerializeField] GameObject _ammoIndicator;

    [Space(10)]
    [SerializeField] GameObject _ammoCurrent;
    [SerializeField] GameObject _ammoMax;
    [SerializeField] GameObject _reloadBar;


    void Update()
    {
        if (_hand.transform.childCount > 0)
        {
            Weapon playerWeapon = _hand.transform.GetChild(0).GetComponent<Weapon>();

            _ammoIndicator.SetActive(true);

            _ammoCurrent.GetComponent<Text>().text      = playerWeapon.GetAmmoCurrent().ToString();
            _ammoMax.GetComponent<Text>().text          = "/" + playerWeapon.GetAmmoMax().ToString();
            _reloadBar.GetComponent<Image>().fillAmount = playerWeapon.GetReloadProgress();
        }

        else
            _ammoIndicator.SetActive(false);
    }
}
