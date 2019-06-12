using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerColorManager : MonoBehaviour
{
    public Color P1Light;
    public Color P1Dark;
    public Color P2Light;
    public Color P2Dark;
    public Color P3Light;
    public Color P3Dark;
    public Color P4Light;
    public Color P4Dark;

    // Sets the colors of the given player object to the colors associated with the given player number
    public void SetColors(GameObject player, InputManager.PlayerNumber pNum) {
        SpriteRenderer bodySprite = player.GetComponent<SpriteRenderer>();
        SpriteRenderer directionSprite = player.transform.Find("DirectionIndicator").gameObject.GetComponent<SpriteRenderer>();
        Image chargeBar = player.GetComponentInChildren<Image>();
        TrailRenderer[] trails = player.GetComponentsInChildren<TrailRenderer>(true);
        Text[] texts = player.GetComponentsInChildren<Text>();

        Color light, dark;

        switch (pNum) {
            case InputManager.PlayerNumber.ONE:
                light = P1Light;
                dark = P1Dark;
                break;
            case InputManager.PlayerNumber.TWO:
                light = P2Light;
                dark = P2Dark;
                break;
            case InputManager.PlayerNumber.THREE:
                light = P3Light;
                dark = P3Dark;
                break;
            case InputManager.PlayerNumber.FOUR:
                light = P4Light;
                dark = P4Dark;
                break;
            default:
                light = Color.white;
                dark = Color.black;
                break;
        }

        GradientColorKey c1 = new GradientColorKey(dark, 0f);
        GradientColorKey c2 = new GradientColorKey(light, 1f);
        GradientColorKey[] colors = new GradientColorKey[] { c1, c2 };

        GradientAlphaKey a1 = new GradientAlphaKey(1f, 0f);
        GradientAlphaKey a2 = new GradientAlphaKey(0.5f, 0f);
        GradientAlphaKey[] alphas = new GradientAlphaKey[] { a1, a2 };

        Gradient g = new Gradient();
        g.SetKeys(colors, alphas);

        bodySprite.color = light;
        directionSprite.color = light;
        chargeBar.color = dark;
        foreach (TrailRenderer t in trails) {
            t.colorGradient = g;
        }
        foreach (Text t in texts) {
            t.color = dark;
        }
    }

    public Color GetPlayerColor(bool light, int pNum) {
        if (light) {
            switch (pNum) {
                case 1:
                    return P1Light;
                case 2:
                    return P2Light;
                case 3:
                    return P3Light;
                case 4:
                    return P4Light;
                default:
                    return Color.white;
            }
        } else {
            switch (pNum) {
                case 1:
                    return P1Dark;
                case 2:
                    return P2Dark;
                case 3:
                    return P3Dark;
                case 4:
                    return P4Dark;
                default:
                    return Color.black;
            }
        }
    }
}
