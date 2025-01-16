using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStatus : UIBase
{
    [SerializeField] private Image _hpBar;
    [SerializeField] private Text _ammo;
    private Player Player;


    private void Start()
    {
        Player = FindFirstObjectByType<Player>();
    }

    private void Update()
    {
       _hpBar.fillAmount = (float)Player.CurrentHP / Player.MaxHP;
        _ammo.text = $"{Player.Weapon.CurrentAmmo} / {Player.CurrentAmmo}";
    }
}
