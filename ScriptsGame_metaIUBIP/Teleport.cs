using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
using Michsky.MUIP;
using Michsky.UI.Heat;
using TMPro;

public class Teleport : MonoBehaviour
{
    [System.Serializable]
    public class TeleportPoint
    {
        public string buttonText;
        public Transform destination;
        public int floorNumber;
    }

    [Header("Teleport Settings")]
    public List<TeleportPoint> teleportPoints = new List<TeleportPoint>();
    public int triggerFloor;
    public int currentFloor;

    [Header("UI References")]
    public GameObject teleportMenu;
    public GameObject buttonPrefab;
    public Transform buttonContainer;

    [Header("UI Customization")]
    public float buttonSpacing = 10f;
    public Vector2 buttonSize = new Vector2(200f, 200f);

    private GameObject currentPlayer;
    private bool isPlayerInTrigger = false;

    private void Start()
    {
        currentPlayer = GameObject.FindGameObjectWithTag("Player");

        if (currentPlayer == null)
        {
            Debug.LogError("Игрок (Player) не найден! Убедитесь, что объект игрока имеет тег 'Player'.");
        }

        ValidateComponents();

        if (teleportMenu != null)
        {
            teleportMenu.SetActive(false);
        }

        SetupUI();
        SetupTeleportButtons();
    }

    private void Update()
    {
        if (isPlayerInTrigger)
        {
            for (int i = 1; i <= 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    int targetFloor = i;
                    TeleportPoint targetPoint = teleportPoints.Find(point => point.floorNumber == targetFloor);
                    
                    if (targetPoint != null)
                    {
                        TryTeleportPlayer(targetPoint);
                    }
                }
            }
        }
    }

    private void ValidateComponents()
    {
        if (teleportMenu == null)
        {
            Debug.LogError("Teleport: teleportMenu не назначен! Назначьте UI панель в инспекторе.");
        }

        if (buttonPrefab == null)
        {
            Debug.LogError("Teleport: buttonPrefab не назначен! Назначьте префаб кнопки в инспекторе.");
        }

        if (buttonContainer == null)
        {
            Debug.LogError("Teleport: buttonContainer не назначен! Назначьте Transform контейнера для кнопок в инспекторе.");
        }

        if (teleportPoints.Count == 0)
        {
            Debug.LogWarning("Teleport: список точек телепортации пуст! Добавьте точки телепортации в инспекторе.");
        }
        else
        {
            foreach (var point in teleportPoints)
            {
                if (point.destination == null)
                {
                    Debug.LogError($"Teleport: точка назначения для кнопки '{point.buttonText}' не назначена!");
                }
            }
        }
    }

    private void SetupUI()
    {
        if (buttonContainer != null)
        {
            RectTransform containerRect = buttonContainer.GetComponent<RectTransform>();
            if (containerRect != null)
            {
                containerRect.anchorMin = new Vector2(0, 0);
                containerRect.anchorMax = new Vector2(1, 1);
                containerRect.sizeDelta = Vector2.zero;
                containerRect.anchoredPosition = Vector2.zero;
            }

            var existingVertical = buttonContainer.GetComponent<VerticalLayoutGroup>();
            var existingHorizontal = buttonContainer.GetComponent<HorizontalLayoutGroup>();
            if (existingVertical != null) DestroyImmediate(existingVertical);
            if (existingHorizontal != null) DestroyImmediate(existingHorizontal);

            var verticalLayout = buttonContainer.gameObject.AddComponent<VerticalLayoutGroup>();
            verticalLayout.spacing = buttonSpacing;
            verticalLayout.childAlignment = TextAnchor.LowerLeft;
            verticalLayout.childControlWidth = false;
            verticalLayout.childControlHeight = false;
            verticalLayout.childForceExpandWidth = false;
            verticalLayout.childForceExpandHeight = false;
            verticalLayout.padding = new RectOffset(10, 10, 10, 10);
        }
    }

    private void SetupTeleportButtons()
    {
        if (buttonContainer == null || buttonPrefab == null)
        {
            Debug.LogError("buttonContainer или buttonPrefab не назначены!");
            return;
        }

        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var point in teleportPoints)
        {
            if (point.destination == null)
            {
                Debug.LogWarning($"Точка назначения для {point.buttonText} не указана!");
                continue;
            }

            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            if (buttonObj == null)
            {
                Debug.LogError("Кнопка не была создана!");
                continue;
            }

            Michsky.MUIP.ButtonManager buttonManager = buttonObj.GetComponent<Michsky.MUIP.ButtonManager>();
            if (buttonManager != null)
            {
                buttonManager.SetText(point.buttonText);
                buttonManager.Interactable(point.floorNumber != currentFloor);

                TeleportPoint pointCopy = point;
                buttonManager.onClick.AddListener(() => TryTeleportPlayer(pointCopy));
            }
            else
            {
                Debug.LogError("ButtonManager не найден на кнопке!");
            }
        }
    }

    private void TryTeleportPlayer(TeleportPoint point)
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("Телепортация невозможна: currentPlayer == null");
            return;
        }

        if (point.destination == null)
        {
            Debug.LogWarning($"Телепортация невозможна: не указана точка назначения для этажа {point.floorNumber}");
            return;
        }

        Debug.Log($"Телепортация на позицию: {point.destination.position}");
        currentPlayer.transform.position = point.destination.position;

        // Настройка масштаба игрока в зависимости от этажа
        if (point.floorNumber == 3)
        {
            currentPlayer.transform.localScale = new Vector3(2.9f, 2.9f, 2.9f);
        }
        else if (point.floorNumber == 4 || point.floorNumber == 5)
        {
            currentPlayer.transform.localScale = new Vector3(2.7f, 2.7f, 2.7f);
        }
        else
        {
            currentPlayer.transform.localScale = new Vector3(2f, 2f, 2f);
        }

        currentFloor = point.floorNumber;
        Debug.Log($"Текущий этаж обновлен на: {currentFloor}");

        SetupTeleportButtons();
        HideTeleportMenu();
    }

    private void ShowTeleportMenu()
    {
        if (teleportMenu != null)
        {
            teleportMenu.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void HideTeleportMenu()
    {
        if (teleportMenu != null)
        {
            teleportMenu.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                currentPlayer = other.gameObject;
                isPlayerInTrigger = true;
                currentFloor = triggerFloor;
                SetupTeleportButtons();
                ShowTeleportMenu();
                Debug.Log($"Локальный игрок вошел в зону телепортации на этаже {triggerFloor}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                currentPlayer = null;
                isPlayerInTrigger = false;
                HideTeleportMenu();
                Debug.Log("Локальный игрок вышел из зоны телепортации");
            }
        }
    }
}