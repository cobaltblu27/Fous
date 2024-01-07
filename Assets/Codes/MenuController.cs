using Codes.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    [SerializeField] private InputActionProperty openMenu;
    [SerializeField] private InputActionProperty leftHandMove;
    [SerializeField] private InputActionProperty selectEntry;
    
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject[] menuObjects;
    [SerializeField] private UnityEvent[] menuEvents;

    private string[] menuItems;
    private bool _menuOpen = false;


    private int _menuIdx;

    private int menuIdx
    {
        get => _menuIdx;
        set
        {
            menuObjects[menuIdx].GetComponent<TMP_Text>().color = Color.white;
            menuObjects[value].GetComponent<TMP_Text>().color = Color.yellow;
            _menuIdx = value;
        }
    }

    private bool _menuMoveCooldown = false;

    void Start()
    {
        menuItems = new string[menuObjects.Length];

        openMenu.action.performed += ToggleMenu;
        leftHandMove.action.performed += MenuMoveAction;
        selectEntry.action.performed += SelectMenuEntry;

        menuObjects[menuIdx].GetComponent<TMP_Text>().color = Color.yellow;
    }

    void ToggleMenu(InputAction.CallbackContext ctx)
    {
        _menuOpen = !_menuOpen;
        menuUI.SetActive(_menuOpen);
    }

    private void MenuMoveAction(InputAction.CallbackContext ctx)
    {
        if (!_menuOpen || _menuMoveCooldown) return;
        var input = ctx.ReadValue<Vector2>();
        if (input.y > 0.5f)
        {
            menuIdx = MathUtils.Mod(menuIdx - 1, menuObjects.Length);
        }
        else if (input.y < -0.5f)
        {
            menuIdx = MathUtils.Mod(menuIdx + 1, menuObjects.Length);
        }
        else
        {
            return;
        }

        _menuMoveCooldown = true;
        Invoke(nameof(MenuCoolDown), 0.3f);
    }

    void SelectMenuEntry(InputAction.CallbackContext ctx)
    {
        if (!_menuOpen) return;

        var menuEvent = menuEvents[_menuIdx];
        menuEvent.Invoke();
        
        _menuOpen = false;
        menuUI.SetActive(false);
    }

    void MenuCoolDown()
    {
        _menuMoveCooldown = false;
    }
}