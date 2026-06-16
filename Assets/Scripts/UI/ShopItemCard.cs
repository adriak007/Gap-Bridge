using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopItemCard : MonoBehaviour
{
    [SerializeField] public Image[]     colorParts;      // head + body do mini-personagem
    [SerializeField] public TMP_Text    nameText;
    [SerializeField] public TMP_Text    statusText;
    [SerializeField] public Button      actionButton;
    [SerializeField] public TMP_Text    buttonLabel;
    [SerializeField] public Image       cardBackground;

    private ShopSkin          skin;
    private Action<ShopSkin>  onBuy;
    private Action<ShopSkin>  onEquip;
    private bool              unlocked;
    private bool              equipped;

    public void Setup(ShopSkin skin, bool unlocked, bool equipped,
                      Action<ShopSkin> onBuy, Action<ShopSkin> onEquip)
    {
        this.skin     = skin;
        this.unlocked = unlocked;
        this.equipped = equipped;
        this.onBuy    = onBuy;
        this.onEquip  = onEquip;

        if (colorParts != null)
            foreach (var part in colorParts)
                if (part) part.color = skin.color;
        if (nameText)     nameText.text      = skin.name;

        if (unlocked)
        {
            if (statusText)    statusText.text            = equipped ? "EQUIPADO" : "DESBLOQ.";
            if (statusText)    statusText.color           = equipped
                                   ? new Color32(0, 220, 100, 255)
                                   : new Color32(150, 150, 150, 255);
            if (buttonLabel)   buttonLabel.text           = equipped ? "EQUIPADO" : "EQUIPAR";
            if (actionButton)  actionButton.interactable  = !equipped;
            if (cardBackground) cardBackground.color      = equipped
                                   ? new Color32(20, 60, 40, 255)
                                   : new Color32(28, 32, 55, 255);
        }
        else
        {
            if (statusText)    statusText.text            = skin.price + " moedas";
            if (statusText)    statusText.color           = new Color32(255, 215, 0, 255);
            if (buttonLabel)   buttonLabel.text           = "COMPRAR";
            if (actionButton)  actionButton.interactable  = true;
            if (cardBackground) cardBackground.color      = new Color32(28, 32, 55, 255);
        }
    }

    public void OnActionClick()
    {
        if (unlocked)
        {
            if (onEquip != null) onEquip(skin);
        }
        else
        {
            if (onBuy != null) onBuy(skin);
        }
    }
}
