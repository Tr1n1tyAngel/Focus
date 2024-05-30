using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject profilePanel;
    public GameObject testPanel;
    public GameObject endPanel;
    public string playerName;
    public string playerSurname;
    public string playerYearOfBirth;
    public bool testStart=false;
    public AudioSource dogBark;
    public AudioSource doorKnock;
    public AudioSource shout;
    public AudioSource garbageTruck;
    public float eventOccur = 5;
    
    
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
    public RectTransform canvasRectTransform;
    private bool isMoving = false;

    //Test screen variables
    public Image profileImage;
    public TMP_InputField profileName;
    public TMP_InputField profileSurname;
    public TMP_InputField profileYOB;
    public TMP_InputField question1;
    public TMP_InputField question2;
    public TMP_InputField question3;
    public TMP_InputField question4;
    public TMP_InputField question5;
    public TMP_InputField question6;
    public TMP_InputField question7;
    public GameObject[] panelPrefab = new GameObject[5]; 
    public Canvas canvas; 
    private List<GameObject> spawnedPanels = new List<GameObject>();
    public GameObject dogPanel;
    public GameObject doorKnockPanel;
    public GameObject garbageTruckPanel;
    public GameObject shoutPanel;

    public GameObject[] valid = new GameObject[11];
    public bool[] validCheck = new bool[11];

    // Timer variables
    public TMP_Text timerText;
    public bool startTimer = false;
    private float timerDuration = 300f; // 5 minutes in seconds
    private float timerRemaining;

    void Start()
    {
        AudioManager.Instance.PlayMusic(0);
        AudioManager.Instance.musicVolume=0.1f;
        startButtonRectTransform = startButton.GetComponent<RectTransform>();
        endPanel.SetActive(false);
        menuPanel.SetActive(true);
        profilePanel.SetActive(false);
        testPanel.SetActive(false);
        
        isMoving = false;
        foreach(GameObject gameobject in valid)
        {
            gameobject.SetActive(false);
        }
        SetRandomDirection();
        startButtonDirectionChangeTimer = startButtonDirectionChangeInterval;
        if (characterSprites.Count > 0)
        {
            characterImage.sprite = characterSprites[currentImageIndex];
        }

        // Initialize timer
        timerRemaining = timerDuration;
        UpdateTimerDisplay();
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

        // Update the timer
        if (timerRemaining > 1f&& startTimer==true)
        {
            timerRemaining -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        if(timerRemaining <= 0f)
        {
            eventOccur = 10f;
        }
        if(testStart)
        {
            InvokeRepeating("InvokeRandomTestChange", 0f, 5f);
            InvokeRepeating("InvokeRandomPopup", 0f, 3f);
            testStart = false;
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timerRemaining / 60F);
        int seconds = Mathf.FloorToInt(timerRemaining % 60F);
        timerText.text = $"Time Remaining: {minutes:00}:{seconds:00}";
    }

    public void gameStart()
    {
        AudioManager.Instance.PlaySFX(3);
        if (!isMoving)
        {
            isMoving = true;
        }
        else if (isMoving)
        {
            menuPanel.SetActive(false);
            profilePanel.SetActive(true);
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayMusic(1);
            AudioManager.Instance.musicVolume = 1f;
            isMoving = false;
        }
    }

    public void gameRestart()
    {
        for (int i = 0; i <= validCheck.Length - 1; i++)
        {
            validCheck[i] = false;
        }
        endPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Quit()
    {
        AudioManager.Instance.PlaySFX(3);
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
        AudioManager.Instance.PlaySFX(3);
        if (characterSprites.Count == 0) return;
        if (profilePanel.activeSelf)
        {
            currentImageIndex = (currentImageIndex - 1 + characterSprites.Count) % characterSprites.Count;
            characterImage.sprite = characterSprites[currentImageIndex];
        }
        if (testPanel.activeSelf)
        {
            currentImageIndex = (currentImageIndex - 1 + characterSprites.Count) % characterSprites.Count;
            profileImage.sprite = characterSprites[currentImageIndex];
        }
        
    }

    public void ShowNextImage()
    {
        AudioManager.Instance.PlaySFX(3);
        if (characterSprites.Count == 0) return;
        if(profilePanel.activeSelf)
        {
            currentImageIndex = (currentImageIndex + 1) % characterSprites.Count;
            characterImage.sprite = characterSprites[currentImageIndex];
        }
        if(testPanel.activeSelf)
        {
            currentImageIndex = (currentImageIndex + 1) % characterSprites.Count;
            profileImage.sprite = characterSprites[currentImageIndex];
        }
        
    }

    public void SaveProfile()
    {
        AudioManager.Instance.PlaySFX(3);
        playerName = nameInputField.text;
        playerSurname = surnameInputField.text;
        playerYearOfBirth = yearOfBirthInputField.text;
        profilePanel.SetActive(false);
        testPanel.SetActive(true);
        profileName.text = playerName;
        profileSurname.text = playerSurname;
        profileYOB.text = playerYearOfBirth;
        playerChosenImage = characterSprites[currentImageIndex];
        profileImage.sprite = playerChosenImage;
        startTimer = true;
        testStart=true;

        Debug.Log($"Profile Saved: {playerName} {playerSurname}, Born in {playerYearOfBirth}");
    }
    void InvokeRandomTestChange()
    {
        int randomIndex = Random.Range(0, 3);
        switch (randomIndex)
        {
            case 0:
                SwitchProfileImage();
                Debug.Log("imagechange");
                break;
            case 1:
                ChangeProfileDetails();
                Debug.Log("profileChange");
                break;
            case 2:
                ClearQuestion();
                Debug.Log("questionchange");
                break;
            
        }
    }
    void InvokeRandomPopup()
    {
        int randomIndex = Random.Range(0, 2);
        switch (randomIndex)
        {
            case 0:
                Advert();
                Debug.Log("advertpopup");
                break;
            case 1:
                AnnoyingSound();
                Debug.Log("annoyingsound");
                break;
        }
    }
    void SwitchProfileImage()
    {
        int randomIndex = Random.Range(0, 4);
        if (profileImage.sprite != characterSprites[randomIndex])
        {
            
            profileImage.sprite = characterSprites[randomIndex];
        }
        else
        {
            SwitchProfileImage();
        }
    }
    void ChangeProfileDetails()
    {

        int rnd = Random.Range(0, 3);
        switch(rnd)
        {
            case 0:
                if (profileName.text.Length > 0)
                {
                    int randomIndex = Random.Range(0, profileName.text.Length);
                    char randomLetter = (char)('a' + Random.Range(0, 26));
                    char[] textArray = profileName.text.ToCharArray();
                    textArray[randomIndex] = randomLetter;
                    profileName.text = new string(textArray);
                }
                break;
            case 1:
                if (profileSurname.text.Length > 0)
                {
                    int randomIndex = Random.Range(0, profileSurname.text.Length);
                    char randomLetter = (char)('a' + Random.Range(0, 26));
                    char[] textArray = profileSurname.text.ToCharArray();
                    textArray[randomIndex] = randomLetter;
                    profileSurname.text = new string(textArray);
                }
                break; 
            case 2:
                if (profileYOB.text.Length > 0)
                {
                    int randomIndex = Random.Range(0, profileYOB.text.Length);

                    // Ensure the character at randomIndex is a digit before replacing
                    while (!char.IsDigit(profileYOB.text[randomIndex]))
                    {
                        randomIndex = Random.Range(0, profileYOB.text.Length);
                    }

                    char randomDigit = (char)('0' + Random.Range(0, 10)); // Generates a random digit (0-9)
                    char[] textArray = profileYOB.text.ToCharArray();
                    textArray[randomIndex] = randomDigit;
                    profileYOB.text = new string(textArray);
                }
                break;
        }
        
    }
    void ClearQuestion()
    {
        int rnd = Random.Range(0, 7);
        switch(rnd)
        {
            case 0:
                question1.text = "";
                break; 
            case 1:
                question2.text = "";
                break; 
            case 2:
                question3.text = "";
                break;
            case 3:
                question4.text = "";
                break;
            case 4:
                question5.text = "";
                break;
            case 5:
                question6.text = "";
                break;
            case 6:
                question7.text = "";
                break;
        }
        
    }
    void Advert()
    {
        AudioManager.Instance.PlaySFX(2);
        if (panelPrefab != null && canvas != null)
        {
            // Instantiate the panel prefab

            GameObject spawnedPanel=null;
            GameObject spawnedPanel1 = null;
            GameObject spawnedPanel2 = null;
            GameObject spawnedPanel3 = null;
            GameObject spawnedPanel4 = null;
            int rnd = Random.Range(0, 5);
            switch(rnd)
            {
                case 0:
                    spawnedPanel = Instantiate(panelPrefab[0]);
                    // Attach it to the canvas
                    spawnedPanel.transform.SetParent(canvas.transform, false);

                    // Set the position to a random location within the screen bounds
                    RectTransform rectTransform = spawnedPanel.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(
                        Random.Range(0, 1400) - 1400 / 2,
                        Random.Range(0, 800) - 800 / 2

                    );
                    // Add the spawned panel to the list
                    spawnedPanels.Add(spawnedPanel);

                    // Optionally, add a button to the panel to close itself
                    Button closeButton = spawnedPanel.GetComponentInChildren<Button>();
                    if (closeButton != null)
                    {
                        closeButton.onClick.AddListener(() => CloseAdvert(spawnedPanel));
                    }
                    break;
                case 1:
                    spawnedPanel1 = Instantiate(panelPrefab[1]);
                    // Attach it to the canvas
                    spawnedPanel1.transform.SetParent(canvas.transform, false);

                    // Set the position to a random location within the screen bounds
                    RectTransform rectTransform1 = spawnedPanel1.GetComponent<RectTransform>();
                    rectTransform1.anchoredPosition = new Vector2(
                        Random.Range(0, 1400) - 1400 / 2,
                        Random.Range(0, 800) - 800 / 2

                    );
                    // Add the spawned panel to the list
                    spawnedPanels.Add(spawnedPanel1);

                    // Optionally, add a button to the panel to close itself
                    Button closeButton1 = spawnedPanel1.GetComponentInChildren<Button>();
                    if (closeButton1 != null)
                    {
                        closeButton1.onClick.AddListener(() => CloseAdvert(spawnedPanel1));
                    }
                    break;
                case 2:
                    spawnedPanel2 = Instantiate(panelPrefab[2]);
                    // Attach it to the canvas
                    spawnedPanel2.transform.SetParent(canvas.transform, false);

                    // Set the position to a random location within the screen bounds
                    RectTransform rectTransform2 = spawnedPanel2.GetComponent<RectTransform>();
                    rectTransform2.anchoredPosition = new Vector2(
                        Random.Range(0, 1400) - 1400 / 2,
                        Random.Range(0, 800) - 800 / 2

                    );
                    // Add the spawned panel to the list
                    spawnedPanels.Add(spawnedPanel2);

                    // Optionally, add a button to the panel to close itself
                    Button closeButton2 = spawnedPanel2.GetComponentInChildren<Button>();
                    if (closeButton2 != null)
                    {
                        closeButton2.onClick.AddListener(() => CloseAdvert(spawnedPanel2));
                    }
                    break;
                case 3:
                    spawnedPanel3 = Instantiate(panelPrefab[3]);
                    // Attach it to the canvas
                    spawnedPanel3.transform.SetParent(canvas.transform, false);

                    // Set the position to a random location within the screen bounds
                    RectTransform rectTransform3 = spawnedPanel3.GetComponent<RectTransform>();
                    rectTransform3.anchoredPosition = new Vector2(
                        Random.Range(0, 1400) - 1400 / 2,
                        Random.Range(0, 800) - 800 / 2

                    );
                    // Add the spawned panel to the list
                    spawnedPanels.Add(spawnedPanel3);

                    // Optionally, add a button to the panel to close itself
                    Button closeButton3 = spawnedPanel3.GetComponentInChildren<Button>();
                    if (closeButton3 != null)
                    {
                        closeButton3.onClick.AddListener(() => CloseAdvert(spawnedPanel3));
                    }
                    break;
                case 4:
                    spawnedPanel4 = Instantiate(panelPrefab[4]);
                    // Attach it to the canvas
                    spawnedPanel4.transform.SetParent(canvas.transform, false);

                    // Set the position to a random location within the screen bounds
                    RectTransform rectTransform4 = spawnedPanel4.GetComponent<RectTransform>();
                    rectTransform4.anchoredPosition = new Vector2(
                        Random.Range(0, 1400) - 1400 / 2,
                        Random.Range(0, 800) - 800 / 2

                    );
                    // Add the spawned panel to the list
                    spawnedPanels.Add(spawnedPanel4);

                    // Optionally, add a button to the panel to close itself
                    Button closeButton4 = spawnedPanel4.GetComponentInChildren<Button>();
                    if (closeButton4 != null)
                    {
                        closeButton4.onClick.AddListener(() => CloseAdvert(spawnedPanel4));
                    }
                    break;
            }
            

            
        }
    }
    public void CloseAdvert(GameObject panel)
    {
        AudioManager.Instance.PlaySFX(3);
        if (panel != null)
        {
            spawnedPanels.Remove(panel);
            Destroy(panel);
        }
    }
    void AnnoyingSound()
    {
        int rnd = Random.Range(0, 4);
        switch(rnd)
        {
            case 0:
                if(!dogPanel.activeSelf)
                {
                    dogPanel.SetActive(true);
                    if (dogBark != null)
                    {
                        dogBark.Play();
                    }
                }
                else
                {
                    AnnoyingSound();
                }
                break;
            case 1:
                if (!doorKnockPanel.activeSelf)
                {
                    doorKnockPanel.SetActive(true);
                    if (doorKnock != null)
                    {
                        doorKnock.Play();
                    }
                }
                break; 
            case 2:
                if (!shoutPanel.activeSelf)
                {
                    shoutPanel.SetActive(true);
                    if (shout != null)
                    {
                        shout.Play();
                    }
                }
                break; 
            case 3:
                if (!garbageTruckPanel.activeSelf)
                {
                    garbageTruckPanel.SetActive(true);
                    if (garbageTruck != null)
                    {
                        garbageTruck.Play();
                    }
                }
                break;
        }
       
        
    }
    public void dogClose()
    {
        AudioManager.Instance.PlaySFX(3);
        dogPanel.SetActive(false);
        dogBark.Stop();
    }
    public void doorClose()
    {
        AudioManager.Instance.PlaySFX(3);
        doorKnockPanel.SetActive(false);
        doorKnock.Stop();
    }
    public void shoutClose()
    {
        AudioManager.Instance.PlaySFX(3);
        shoutPanel.SetActive(false);
        shout.Stop();
    }
    public void garbageClose()
    {
        AudioManager.Instance.PlaySFX(3);
        garbageTruckPanel.SetActive(false);
        garbageTruck.Stop();
    }
    public void Submit()
    {
        AudioManager.Instance.PlaySFX(3);
        if (profileImage.sprite != playerChosenImage)
        {
            valid[0].SetActive(true);
        }
        else
        {
            validCheck[0]=true;
        }
        if (profileName.text != playerName)
        {
            valid[1].SetActive(true);
        }
        else
        {
            validCheck[1] = true;
        }
        if (profileSurname.text != playerSurname)
        {
            valid[2].SetActive(true);
        }
        else
        {
            validCheck[2] = true;
        }
        if (profileYOB.text != playerYearOfBirth)
        {
            valid[3].SetActive(true);
        }
        else
        {
            validCheck[3] = true;
        }
        if(question1.text== "")
        {
            valid[4].SetActive(true);
        }
        else
        {
            validCheck[4] = true;
        }
        if (question2.text== "")
        {
            valid[5].SetActive(true);
        }
        else
        {
            validCheck[5] = true;
        }
        if (question3.text == "")
        {
            valid[6].SetActive(true);
        }
        else
        {
            validCheck[6] = true;
        }
        if (question4.text == "")
        {
            valid[7].SetActive(true);
        }
        else
        {
            validCheck[7] = true;
        }
        if (question5.text == "")
        {
            valid[8].SetActive(true);
        }
        else
        {
            validCheck[8] = true;
        }
        if (question6.text =="")
        {
            valid[9].SetActive(true);
        }
        else
        {
            validCheck[9] = true;
        }
        if (question7.text == "")
        {
            valid[10].SetActive(true);
        }
        else
        {
            validCheck[10] = true;
        }

        bool allTrue = validCheck.All(b => b);
        if(allTrue)
        {
            CancelInvoke();
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayMusic(0);
            AudioManager.Instance.musicVolume=0.1f;
            testPanel.SetActive(false);
            dogPanel.SetActive(false);
            doorKnockPanel.SetActive(false);
            shoutPanel.SetActive(false);
            garbageTruckPanel.SetActive(false);
            endPanel.SetActive(true);

            Debug.Log("Test Completed");
        }
        else
        {
            StartCoroutine(ResetValid());
        }
    }
    private IEnumerator ResetValid() 
    { 
        yield return new WaitForSecondsRealtime(3);
        foreach(GameObject gameObject in valid)
        {
            gameObject.SetActive(false);
        }
        for(int i= 0; i<= validCheck.Length-1; i++)
        {
            validCheck[i] = false;
        }
        
    }
}