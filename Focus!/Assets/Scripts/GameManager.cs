using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject profilePanel;
    public GameObject testPanel;
    //Profile screen variables
    public Image characterImage;
    public Sprite playerChosenImage;
    public List<Sprite> characterSprites; // List of character sprites
    public Button previousButton;
    public Button nextButton;
    public TMP_InputField nameInputField;
    public TMP_InputField surnameInputField;
    public TMP_InputField yearOfBirthInputField;
    public Button saveButton;
    private int currentImageIndex = 0;
    //Button moving variables
    public Button startButton;
    public float startButtonMoveSpeed = 100f;
    public float startButtonDirectionChangeInterval = 1f;
    private Vector2 startButtonCurrentDirection;
    private float startButtonDirectionChangeTimer;
    private RectTransform startButtonRectTransform;
    private RectTransform canvasRectTransform;
    private bool isMoving = false;

    //Test screen variables


    void Start()
    {
        startButtonRectTransform = startButton.GetComponent<RectTransform>();
        canvasRectTransform = startButton.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        menuPanel.SetActive(true);
        profilePanel.SetActive(false);
        testPanel.SetActive(false);
        isMoving = false;
        SetRandomDirection();
        startButtonDirectionChangeTimer = startButtonDirectionChangeInterval;
        if (characterSprites.Count > 0)
        {
            characterImage.sprite = characterSprites[currentImageIndex];
        }
    }

    void Update()
    {
        if (isMoving)
        {
            startButtonDirectionChangeTimer -= Time.deltaTime;
            if (startButtonDirectionChangeTimer <= 0f)
            {
                SetRandomDirection();
                startButtonDirectionChangeTimer = startButtonDirectionChangeInterval;
            }
            MoveButton();
        }
    }


    public void gameStart()
    {
        if (!isMoving)
        {
            isMoving = true;
        }
        else if (isMoving)
        {
            menuPanel.SetActive(false);
            profilePanel.SetActive(true);
            isMoving = false;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
    //Button Move Methods
    void SetRandomDirection()
    {
        startButtonCurrentDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
    void MoveButton()
    {
        // Calculate the new position
        Vector2 newPosition = startButtonRectTransform.anchoredPosition;
        newPosition += startButtonCurrentDirection * startButtonMoveSpeed * Time.deltaTime;

        // Clamp the position to stay within the screen bounds considering the button's size
        float halfWidth = startButtonRectTransform.rect.width / 2;
        float halfHeight = startButtonRectTransform.rect.height / 2;

        float canvasWidth = canvasRectTransform.rect.width;
        float canvasHeight = canvasRectTransform.rect.height;

        newPosition.x = Mathf.Clamp(newPosition.x, -canvasWidth / 2 + halfWidth, canvasWidth / 2 - halfWidth);
        newPosition.y = Mathf.Clamp(newPosition.y, -canvasHeight / 2 + halfHeight, canvasHeight / 2 - halfHeight);

        // Apply the new position
        startButtonRectTransform.anchoredPosition = newPosition;
    }
    //Profile Screen Methods
    public void ShowPreviousImage()
    {
        if (characterSprites.Count == 0) return;

        currentImageIndex = (currentImageIndex - 1 + characterSprites.Count) % characterSprites.Count;
        characterImage.sprite = characterSprites[currentImageIndex];
    }

    public void ShowNextImage()
    {
        if (characterSprites.Count == 0) return;

        currentImageIndex = (currentImageIndex + 1) % characterSprites.Count;
        characterImage.sprite = characterSprites[currentImageIndex];
    }

    public void SaveProfile()
    {
        string playerName = nameInputField.text;
        string playerSurname = surnameInputField.text;
        string playerYearOfBirth = yearOfBirthInputField.text;
        playerChosenImage = characterSprites[currentImageIndex];
        profilePanel.SetActive(false);
        testPanel.SetActive(true);
       
        Debug.Log($"Profile Saved: {playerName} {playerSurname}, Born in {playerYearOfBirth}");
    }
}