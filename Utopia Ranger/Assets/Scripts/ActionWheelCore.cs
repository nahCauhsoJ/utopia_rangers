using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionWheelCore : MonoBehaviour
{
    public static ActionWheelCore main;

    public Image[] button_imgs = new Image[4]; // Order will be: [Top][Left][Bottom][Right]
    public Image[] button_imgs_nodefense = new Image[1];

    bool in_place_zone;
    DefenseBase inspected_defense;

    void Awake()
    {
        main = this;
        foreach (var i in button_imgs)
        {
            i.alphaHitTestMinimumThreshold = 0.5f;
        }
    }

    void Update()
    {
        ActionModeUpdate();
    }

    void ActionModeUpdate()
    {
        int result_mode = 0; // 0: Nothing shows, 1: Build mode (no defense nearby), 2: Inspect mode (touching a defense)
        Collider2D[] cols = Physics2D.OverlapPointAll(PlayerController.main.transform.position, LayerMask.GetMask("Units","Zone_Placable"));
        
        DefenseBase found_defense = null;
        foreach (var i in cols)
        {
            DefenseBase def = i.GetComponent<DefenseBase>();
            if (def != null) { found_defense = def; break; }
        }
        if (found_defense != null) result_mode = 2;

        if (result_mode == 0)
        {
            bool found_place_zone = false;
            foreach (var i in cols) if (MapCore.main.place_zones.Contains(i)) { found_place_zone = true; break; }
            if (found_place_zone) result_mode = 1;
        }
        

        switch (result_mode)
        {
            case 0:
                if (in_place_zone || button_imgs[0].gameObject.activeSelf)
                {
                    button_imgs[0].gameObject.SetActive(false);
                    button_imgs[1].gameObject.SetActive(false);
                    button_imgs[2].gameObject.SetActive(false);
                    button_imgs[3].gameObject.SetActive(false);
                    button_imgs_nodefense[0].gameObject.SetActive(false);
                    in_place_zone = false;
                }
                transform.position = PlayerController.main.transform.position;
                break;
            case 1:
                if (!in_place_zone)
                {
                    button_imgs[0].gameObject.SetActive(false);
                    button_imgs[1].gameObject.SetActive(false);
                    button_imgs[2].gameObject.SetActive(false);
                    button_imgs[3].gameObject.SetActive(false);
                    button_imgs_nodefense[0].gameObject.SetActive(true);
                    in_place_zone = true;
                }
                transform.position = PlayerController.main.transform.position;
                break;
            case 2:
                transform.position = found_defense.transform.position;
                if (inspected_defense != found_defense)
                {
                    button_imgs[0].gameObject.SetActive(true);
                    button_imgs[1].gameObject.SetActive(true);
                    button_imgs[2].gameObject.SetActive(true);
                    button_imgs[3].gameObject.SetActive(true);
                    button_imgs_nodefense[0].gameObject.SetActive(false);
                }
                break;
        }

        inspected_defense = found_defense;
    }

    public void OnActionSelect(string act)
    {
        switch (act)
        {
            case "build":
                break;
            case "delete":
                break;
            case "move":
                break;
            case "overclock":
                break;
            case "upgrade":
                break;
        }
    }

    void ActionBuild()
    {

    }

    void ActionDelete()
    {
        
    }

    void ActionMove()
    {
        
    }

    void ActionOverclock()
    {
        
    }

    void ActionUpgrade()
    {
        
    }
}
