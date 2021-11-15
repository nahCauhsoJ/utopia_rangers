using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionWheelCore : MonoBehaviour
{
    public static ActionWheelCore main;

    public Image[] button_imgs = new Image[4]; // Order will be: [Top][Left][Bottom][Right]
    public Image[] button_imgs_nodefense = new Image[2];
    [HideInInspector] public Button[] button_btns = new Button[4];
    [HideInInspector] public Button[] button_btns_nodefense = new Button[2];

    public GameObject buildlist_obj;
    public Image occupation_range;
    public Image aim_range;

    public GameObject upgardelist_obj;
    public List<GameObject> upgradelist_unit_lists; // This is parallel to the unit_id.

    bool in_place_zone;
    DefenseBase inspected_defense;
    DefenseCore.DefenseData inspected_defense_data;

    DefenseBase picked_up_defense;

    void Awake()
    {
        main = this;
        for (var i = 0; i < button_imgs.Length; i++)
        {
            button_imgs[i].alphaHitTestMinimumThreshold = 0.5f;
            button_btns[i] = button_imgs[i].GetComponent<Button>();
        }
        for (var i = 0; i < button_imgs_nodefense.Length; i++)
        {
            button_imgs_nodefense[i].alphaHitTestMinimumThreshold = 0.5f;
            button_btns_nodefense[i] = button_imgs_nodefense[i].GetComponent<Button>();
        }
    }

    void Update()
    {
        ActionModeUpdate();
        BuildListUpdate();
    }

    void BuildListUpdate()
    {
        if (occupation_range.enabled)
        {
            occupation_range.gameObject.transform.position =
                aim_range.gameObject.transform.position =
                PlayerController.main.transform.position;
            occupation_range.color = BuildableCheck(Vector2.one * occupation_range.rectTransform.sizeDelta.x / 16f) ?
                new Color (0f,1f,0f,0.5f) : new Color (1f,0f,0f,0.5f);
        }
    }

    void ActionModeUpdate()
    {
        int result_mode = 0; // 0: Nothing shows, 1: Build mode (no defense nearby), 2: Inspect mode (touching a defense)
        Collider2D[] cols = Physics2D.OverlapPointAll(PlayerController.main.transform.position, LayerMask.GetMask("Units"));
        
        DefenseBase found_defense = null;
        foreach (var i in cols)
        {
            DefenseBase def = i.GetComponent<DefenseBase>();
            if (def != null) { found_defense = def; break; }
        }
        if (found_defense != null) result_mode = 2;

        if (result_mode == 0)
        {
            Collider2D col = Physics2D.OverlapPoint(PlayerController.main.transform.position, LayerMask.GetMask("Zone_Placable"));
            if (col != null) result_mode = 1;
        }
        if (buildlist_obj.activeSelf || upgardelist_obj.activeSelf) result_mode = 0;

        switch (result_mode)
        {
            case 0:
                if (in_place_zone || button_imgs[0].gameObject.activeSelf)
                {
                    foreach (var i in button_imgs) i.gameObject.SetActive(false);
                    foreach (var i in button_imgs_nodefense) i.gameObject.SetActive(false);
                    in_place_zone = false;
                }
                transform.position = PlayerController.main.transform.position;
                break;
            case 1:
                if (!in_place_zone || button_imgs[0].gameObject.activeSelf)
                {
                    foreach (var i in button_imgs) i.gameObject.SetActive(false);
                    foreach (var i in button_imgs_nodefense) i.gameObject.SetActive(true);
                    if (button_imgs_nodefense[1] != null) // Just to check if we get the correct buttons
                    {   
                        button_btns_nodefense[0].interactable = picked_up_defense == null;
                        button_btns_nodefense[1].interactable = picked_up_defense != null;
                    }
                    in_place_zone = true;
                }
                transform.position = PlayerController.main.transform.position;
                break;
            case 2:
                transform.position = found_defense.transform.position;
                if (inspected_defense != found_defense)
                {
                    foreach (var i in button_imgs) i.gameObject.SetActive(true);
                    foreach (var i in button_imgs_nodefense) i.gameObject.SetActive(false);
                    if (button_imgs[3] != null) button_btns[0].interactable = found_defense.current_upgrade < found_defense.max_level;
                }
                break;
        }

        if (inspected_defense != null && found_defense == null) upgardelist_obj.SetActive(false);
        inspected_defense = found_defense;
        inspected_defense_data = DefenseCore.GetDefDataFromInstance(inspected_defense);
    }

    public void OnActionSelect(string act)
    {
        switch (act)
        {
            case "build": ActionBuild(); break;
            case "delete": ActionDelete(); break;
            case "move": ActionMove(); break;
            case "overclock": ActionOverclock(); break;
            case "place": ActionPlace(); break;
            case "upgrade": ActionUpgrade(); break;
        }
    }

    void ActionBuild()
    {
        buildlist_obj.SetActive(true);
    }

    // It says Delete, but it's only for salvaging. Units that die by other cause will use another one.
    void ActionDelete()
    {   // Deleted units will have half of its cost salvaged.
        if (inspected_defense_data != null) DefenseCore.main.greed += DefenseCore.GetTotalCost(inspected_defense) / 2;
        Destroy(inspected_defense.gameObject);
    }

    void ActionMove()
    {   // Same cost as deleting units, except the upgrade is kept. More than a bargain, amairite?
        int move_cost = DefenseCore.GetTotalCost(inspected_defense) / 2;
        if (!CostCheck(move_cost)) {
            print(string.Format("Can't afford to move {0}.", inspected_defense_data.unit_name));
        } else {
            DefenseCore.main.greed -= move_cost;
            picked_up_defense = inspected_defense;
            picked_up_defense.gameObject.SetActive(false);
            print(string.Format("Picked up {0}.", inspected_defense_data.unit_name));
        }
    }

    void ActionOverclock()
    {
        
    }

    void ActionPlace()
    {
        picked_up_defense.transform.position = PlayerController.main.transform.position;
        picked_up_defense.gameObject.SetActive(true);
        print(string.Format("Placed {0} back.", DefenseCore.GetDefDataFromInstance(picked_up_defense).unit_name));
        picked_up_defense = null;
    }

    void ActionUpgrade()
    {
        // Just in case. Saw the button being too slow to turn off.
        if (inspected_defense == null || inspected_defense.current_upgrade >= inspected_defense.max_level) return;
        for (var i = 0; i < upgradelist_unit_lists.Count; i++)
            if (inspected_defense_data.unit_id == i) upgradelist_unit_lists[i].SetActive(true);
            else upgradelist_unit_lists[i].SetActive(false);
        upgardelist_obj.SetActive(true);
    }

    // Detailed mechanics for the Build action.
    public void OnActionBuild(int t)
    {
        OnActionBuildHoverOff();
        DefenseCore.DefenseData d = DefenseCore.GetDefData(t);
        if (!BuildableCheck(d.unit_size)) {
            print(string.Format("Can't summon {0} here.",d.unit_name));
        } else if (!CostCheck(d.cost.lvl_0)) {
            print(string.Format("You cannot afford to summon {0}.",d.unit_name));
        } else BuildUnit(t);
        buildlist_obj.SetActive(false);
    }

    public void OnActionBuildHoverOn(int i)
    {
        DefenseCore.DefenseData d = DefenseCore.GetDefData(i);
        occupation_range.rectTransform.sizeDelta = d.unit_size * 16f;
        occupation_range.enabled = true;

        if (d.unit_prefab.transform.childCount > 0)
        {
            float aim_range_radius = d.unit_prefab.transform.GetChild(0).GetComponent<CircleCollider2D>().radius;
            aim_range.rectTransform.sizeDelta = Vector2.one * 32f * aim_range_radius;
            aim_range.enabled = true;
        }
    }

    public void OnActionBuildHoverOff() { occupation_range.enabled = false; aim_range.enabled = false; }

    // Checks if player has enough money
    // Heh, right. Looks trivial, almost wasting space.
    bool CostCheck(int cost)
    {
        if (DefenseCore.main.greed < cost) return false;
        return true;
    }

    // Checks if the unit is fully in placable zone and not overlapping
    bool BuildableCheck(Vector2 unit_size)
    {
        Vector2 hf_unit_size = unit_size * 0.5f;
        Vector2 pos = PlayerController.main.transform.position;
        bool[] conds = new bool[2]; // 0: Touched a 
        Collider2D col = Physics2D.OverlapCircle(pos, hf_unit_size[0],LayerMask.GetMask("Units"));
        if (col != null) return false;
        foreach (var i in new Vector2[]{Vector2.up,Vector2.left,Vector2.down,Vector2.right})
        {
            col = Physics2D.OverlapPoint(pos + i * hf_unit_size, LayerMask.GetMask("Zone_Placable"));
            if (col == null) return false;
        }
        return true;
    }

    void BuildUnit(int unit_id)
    {
        DefenseCore.DefenseData d = DefenseCore.GetDefData(unit_id);
        Instantiate(d.unit_prefab, PlayerController.main.transform.position, Quaternion.identity, DefenseCore.main.transform);
        DefenseCore.main.greed -= d.cost.lvl_0;
        print(string.Format("Summoned {0}.",d.unit_name));
    }

    // Detailed mechanic of Upgrade
    // If future levels are made, we can still use the integer and split it to get multiple values.
    public void OnActionUpgrade(int option)
    {   // To run this function, it's guaranteed to have inspected_defense.
        if (!CostCheck(inspected_defense_data.cost.lvl_1[option])) {
            print(string.Format("Can't afford to upgrade {0} here.", inspected_defense_data.unit_name));
        } else {
            UpgradeUnit(inspected_defense,option);
        }
        upgardelist_obj.SetActive(false);
    }

    void UpgradeUnit(DefenseBase unit_obj, int upgrade_type)
    {
        unit_obj.current_upgrade = 1;
        unit_obj.upgrade_1_type = upgrade_type;
        DefenseCore.main.greed -= inspected_defense_data.cost.lvl_1[upgrade_type];
        unit_obj.UpdateLevel();
        print(string.Format("Upgraded {0}.",DefenseCore.GetDefDataFromInstance(unit_obj).unit_name));
        inspected_defense = null; // Just a hard fix to make the action wheel reappear after upgrade.
    }
}
