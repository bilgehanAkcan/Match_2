using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameLogic : MonoBehaviour
{
    private ItemGridGenerator itemGridGenerator;
    [SerializeField] TextMeshProUGUI movesText;
    [SerializeField] Button goal1;
    [SerializeField] Button goal2;
    [SerializeField] Sprite button1Sprite;
    [SerializeField] Sprite button2Sprite;
    [SerializeField] int goal1Count;
    [SerializeField] int goal2Count;
    [SerializeField] int movesCount;
    TextMeshProUGUI button1Text;
    TextMeshProUGUI button2Text;
    Image buttonImage1;
    Image buttonImage2;
    private AudioSource audioSource;
    private int numberOfItemsDestroyed = 0;

    void Start()
    {
        button1Text = goal1.GetComponentInChildren<TextMeshProUGUI>();
        button2Text = goal2.GetComponentInChildren<TextMeshProUGUI>();
        button1Text.text = "" + goal1Count;
        button2Text.text = "" + goal2Count;
        buttonImage1 = goal1.GetComponent<Image>();
        buttonImage1.sprite = button1Sprite;
        buttonImage2 = goal2.GetComponent<Image>();
        buttonImage2.sprite = button2Sprite;
        itemGridGenerator = GetComponentInChildren<ItemGridGenerator>();
        movesText.text = "" + movesCount;
        if (itemGridGenerator != null)
        {
            itemGridGenerator.OnItemClick += OnItemClick;
        }
    }

    void Update() {
        if (movesCount == 0 || (goal1Count == 0 && goal2Count == 0)) {
            for (int i = 0; i < itemGridGenerator.getAllItems().GetLength(0); i++) {
                for (int j = 0; j < itemGridGenerator.getAllItems().GetLength(1); j++) {
                    if (itemGridGenerator.getItem(i, j) != null) {
                        Button button = itemGridGenerator.getItem(i, j).GetComponent<Button>();
                        if (button != null) {
                            button.interactable = false;
                        }
                    }
                }
            }
        }
    }

    void OnDestroy()
    {
        if (itemGridGenerator != null)
        {
            itemGridGenerator.OnItemClick -= OnItemClick;
        }
    }

    private void OnItemClick(GameObject item)
    {
        Vector2Int itemIndex = itemGridGenerator.getItemIndex(item);
        string itemType = item.tag; 

        bool check1 = false;
        bool check2 = false;
        bool check3 = false;
        bool check4 = false;
        int x = itemIndex.x;
        int y = itemIndex.y;

        if ((x - 1) >= 0 || (x - 1) < itemGridGenerator.getAllItems().GetLength(0) || y >= 0 || y < itemGridGenerator.getAllItems().GetLength(1)) {
            GameObject neighbour1 = itemGridGenerator.getItem(x - 1, y);
            if (neighbour1 != null && neighbour1.tag == itemType) {
                check1 = true;
            }
        }
        if ((x + 1) >= 0 || (x + 1) < itemGridGenerator.getAllItems().GetLength(0) || y >= 0 || y < itemGridGenerator.getAllItems().GetLength(1)) {
            GameObject neighbour2 = itemGridGenerator.getItem(x + 1, y);
            if (neighbour2 != null && neighbour2.tag == itemType) {
                check2 = true;
            }
        }
        if (x >= 0 || x < itemGridGenerator.getAllItems().GetLength(0) || (y - 1) >= 0 || (y - 1) < itemGridGenerator.getAllItems().GetLength(1)) {
            GameObject neighbour3 = itemGridGenerator.getItem(x, y - 1);
            if (neighbour3 != null && neighbour3.tag == itemType) {
                check3 = true;
            }
        }
        if (x >= 0 || x < itemGridGenerator.getAllItems().GetLength(0) || (y + 1) >= 0 || (y + 1) < itemGridGenerator.getAllItems().GetLength(1)) {
            GameObject neighbour4 = itemGridGenerator.getItem(x, y + 1);
            if (neighbour4 != null && neighbour4.tag == itemType) {
                check4 = true;
            }
        }
        if ((check1 || check2 || check3 || check4) || item.tag == "Left Rocket" || item.tag == "Up Rocket") {
            DestroyItemAndNeighborsAndFillGrid(x, y, itemType);
        }
    }

    public void DestroyItemAndNeighborsAndFillGrid(int x, int y, string itemType)
    {
        StartCoroutine(DestroyItemAndNeighborsCoroutine(x, y, itemType));
    }

    public IEnumerator DestroyItemAndNeighborsCoroutine(int x, int y, string itemType)
    {
        DestroyItemAndNeighbors(x, y, itemType);

        float destructionDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < destructionDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (numberOfItemsDestroyed >= 5) {
            itemGridGenerator.fillItemGrid(x, y, true);
        }
        else {
            itemGridGenerator.fillItemGrid(x, y, false);
        }
        numberOfItemsDestroyed = 0;
        movesCount--;
        movesText.text = "" + movesCount;
        button1Text.text = "" + goal1Count;
        button2Text.text = "" + goal2Count;
    }

    private IEnumerator AnimateFallingObject(GameObject fallingObject, Vector2 startPosition, Vector2 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = Mathf.Clamp01(elapsedTime / duration);
            Vector2 newPosition = Vector2.Lerp(startPosition, targetPosition, t);

            fallingObject.GetComponent<RectTransform>().anchoredPosition = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(fallingObject);
    }

    private IEnumerator SpawnAndAnimateFallingObjects(Vector2 spawnPosition, string itemTag)
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject item = Instantiate(itemGridGenerator.getCandyPrefab(9), itemGridGenerator.getItemsRectTransform());
            Image imageItem = item.GetComponent<Image>();
            Color newColor = new Color(1.0f, 1.0f, 1.0f);;
            if (itemTag == "Yellow") {
                newColor = new Color(1.0f, 1.0f, 0.0f);
            }
            else if (itemTag == "Red") {
                newColor = new Color(1.0f, 0.0f, 0.0f);
            }
            else if (itemTag == "Purple") {
                newColor = new Color(0.5f, 0.0f, 0.5f);
            }
            else if (itemTag == "Blue") {
                newColor = new Color(0.0f, 0.0f, 1.0f);
            }
            else if (itemTag == "Green") {
                newColor = new Color(0.0f, 1.0f, 0.0f);
            }
            imageItem.color = newColor;
            RectTransform candyRectTransform = item.GetComponent<RectTransform>();
            candyRectTransform.sizeDelta = new Vector2(itemGridGenerator.getCellSize() / 5, itemGridGenerator.getCellSize() / 5);
            float xOffset = Random.Range(-itemGridGenerator.getCellSize() * 0.25f, itemGridGenerator.getCellSize() * 0.25f);
            Vector2 startPosition = spawnPosition + new Vector2(xOffset, 0);
            Vector2 targetPosition = startPosition - Vector2.up * Random.Range(50, 100);

            StartCoroutine(AnimateFallingObject(item, startPosition, targetPosition, 0.5f));

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator MoveCompletedGoals(Vector2 startingPosition, RectTransform itemRectTransform, Button goal1) {
        audioSource = goal1.GetComponent<AudioSource>();
        if (audioSource != null) {
            audioSource.Play();
        }
        float elapsedTime = 0f;
        RectTransform goal1RectTransform = goal1.GetComponent<RectTransform>();
        Vector2 finalPosition = goal1RectTransform.anchoredPosition;
        while (elapsedTime < 0.5f)
        {
            float t = Mathf.Clamp01(elapsedTime / 0.5f);
            itemRectTransform.anchoredPosition = Vector2.Lerp(startingPosition, finalPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        itemRectTransform.anchoredPosition = finalPosition;
    }

    private IEnumerator WaitForCoroutineAndDestroy(IEnumerator coroutine, GameObject itemToDestroy, float clipLength) {
        yield return StartCoroutine(coroutine);
        if (itemToDestroy != null) {
            Destroy(itemToDestroy, clipLength);
        }
    }

    private IEnumerator MoveRocket(Vector2 leftRocketStartingPosition, Vector2 rightRocketStartingPosition, RectTransform leftRocketRectTransform, RectTransform rightRocketRectTransform, int y) {
        float elapsedTime = 0f;
        Vector2 leftRocketFinalPosition = leftRocketStartingPosition - new Vector2(itemGridGenerator.getCellSize() * y, 0);
        Vector2 rightRocketFinalPosition = rightRocketStartingPosition + new Vector2(itemGridGenerator.getCellSize() * (itemGridGenerator.getAllItems().GetLength(1) - y), 0);
        while (elapsedTime < 0.5f)
        {
            float t = Mathf.Clamp01(elapsedTime / 0.5f);
            leftRocketRectTransform.anchoredPosition = Vector2.Lerp(leftRocketStartingPosition, leftRocketFinalPosition, t);
            rightRocketRectTransform.anchoredPosition = Vector2.Lerp(rightRocketStartingPosition, rightRocketFinalPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        leftRocketRectTransform.anchoredPosition = leftRocketFinalPosition;
        rightRocketRectTransform.anchoredPosition = rightRocketFinalPosition;
    }

    private IEnumerator MoveRocketUp(Vector2 leftRocketStartingPosition, Vector2 rightRocketStartingPosition, RectTransform leftRocketRectTransform, RectTransform rightRocketRectTransform, int x) {
        float elapsedTime = 0f;
        Vector2 leftRocketFinalPosition = leftRocketStartingPosition + new Vector2(0, itemGridGenerator.getCellSize() * x);
        Vector2 rightRocketFinalPosition = rightRocketStartingPosition - new Vector2(0, itemGridGenerator.getCellSize() * (itemGridGenerator.getAllItems().GetLength(0) - x));
        while (elapsedTime < 0.5f)
        {
            float t = Mathf.Clamp01(elapsedTime / 0.5f);
            leftRocketRectTransform.anchoredPosition = Vector2.Lerp(leftRocketStartingPosition, leftRocketFinalPosition, t);
            rightRocketRectTransform.anchoredPosition = Vector2.Lerp(rightRocketStartingPosition, rightRocketFinalPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        leftRocketRectTransform.anchoredPosition = leftRocketFinalPosition;
        rightRocketRectTransform.anchoredPosition = rightRocketFinalPosition;
    }

    private IEnumerator WaitForCoroutineAndDestroy2(IEnumerator coroutine, GameObject itemToDestroy, GameObject rightRocket) {
        yield return StartCoroutine(coroutine);
        if (itemToDestroy != null) {
            Destroy(itemToDestroy);
        }
        if (rightRocket != null) {
            Destroy(rightRocket);
        }
    }

    private void MoveAndExplodeAnotherRocket(GameObject obj, int y) {
        RectTransform leftRocketRectTransform = obj.GetComponent<RectTransform>();
        RocketScript anotherLeftRocketObject = obj.GetComponent<RocketScript>();
        GameObject anotherRightRocket = anotherLeftRocketObject.GetRightRocket();
        RectTransform anotherRightRocketRectTransform = anotherRightRocket.GetComponent<RectTransform>();
        Vector2 leftRocketInitialPosition = leftRocketRectTransform.anchoredPosition;
        Vector2 rightRocketInitialPosition = anotherRightRocketRectTransform.anchoredPosition;
        IEnumerator coroutine = MoveRocket(leftRocketInitialPosition, rightRocketInitialPosition, leftRocketRectTransform, anotherRightRocketRectTransform, y);
        StartCoroutine(WaitForCoroutineAndDestroy2(coroutine, obj, anotherRightRocket));
    }

    private void MoveAndExplodeAnotherRocket2(GameObject obj, int x) {
        RectTransform leftRocketRectTransform = obj.GetComponent<RectTransform>();
        RocketScript anotherLeftRocketObject = obj.GetComponent<RocketScript>();
        GameObject anotherRightRocket = anotherLeftRocketObject.GetRightRocket();
        RectTransform anotherRightRocketRectTransform = anotherRightRocket.GetComponent<RectTransform>();
        Vector2 leftRocketInitialPosition = leftRocketRectTransform.anchoredPosition;
        Vector2 rightRocketInitialPosition = anotherRightRocketRectTransform.anchoredPosition;
        IEnumerator coroutine = MoveRocketUp(leftRocketInitialPosition, rightRocketInitialPosition, leftRocketRectTransform, anotherRightRocketRectTransform, x);
        StartCoroutine(WaitForCoroutineAndDestroy2(coroutine, obj, anotherRightRocket));
    }


    private void DestroyItemAndNeighbors(int x, int y, string itemType) {
        if (x < 0 || x >= itemGridGenerator.getAllItems().GetLength(0) || y < 0 || y >= itemGridGenerator.getAllItems().GetLength(1)) {
            return;
        }

        GameObject currentItem = itemGridGenerator.getItem(x, y);
        if (currentItem != null && currentItem.tag == "Left Rocket" && currentItem.tag == itemType) {
            RocketScript leftRocketObject = currentItem.GetComponent<RocketScript>();
            GameObject rightRocket = leftRocketObject.GetRightRocket();
            for (int i = 0; i < itemGridGenerator.getAllItems().GetLength(1); i++) {
                GameObject itemDestroyed = itemGridGenerator.getItem(x, i);
                if (itemDestroyed.tag == "Left Rocket" && i != y) {
                    MoveAndExplodeAnotherRocket(itemDestroyed, y);
                }
                else if (itemDestroyed.tag == "Up Rocket") {
                    MoveAndExplodeAnotherRocket2(itemDestroyed, x);
                }
                else if (itemDestroyed.tag == "Left Rocket") {
                    RectTransform leftRocketRectTransform = itemDestroyed.GetComponent<RectTransform>();
                    RectTransform rightRocketRectTransform = rightRocket.GetComponent<RectTransform>();
                    Vector2 leftRocketInitialPosition = leftRocketRectTransform.anchoredPosition;
                    Vector2 rightRocketInitialPosition = rightRocketRectTransform.anchoredPosition;
                    IEnumerator coroutine = MoveRocket(leftRocketInitialPosition, rightRocketInitialPosition, leftRocketRectTransform, rightRocketRectTransform, y);
                    StartCoroutine(WaitForCoroutineAndDestroy2(coroutine, itemDestroyed, rightRocket));
                }
                else {
                    audioSource = itemDestroyed.GetComponent<AudioSource>();
                    float clipLength = 0.5f;
                    if (audioSource != null) {
                        audioSource.Play();
                        clipLength = audioSource.clip.length;
                    }
                    
                    Vector2 fallingPosition = itemDestroyed.GetComponent<RectTransform>().anchoredPosition;
                    StartCoroutine(SpawnAndAnimateFallingObjects(fallingPosition, itemDestroyed.tag));
                    if (itemDestroyed.tag == "Balloon" && itemDestroyed.tag == buttonImage1.sprite.name) {
                        if (goal1Count > 0)
                            goal1Count--;
                    }
                    if (itemDestroyed.tag == "Balloon" && itemDestroyed.tag == buttonImage2.sprite.name) {
                        if (goal2Count > 0)
                            goal2Count--;
                    }
                    if (itemDestroyed.tag == buttonImage1.sprite.name && itemDestroyed.tag != "Balloon" && itemDestroyed.tag != "Duck") {
                        MoveAndDestroyItem(itemDestroyed, clipLength);
                    }
                    else if (itemDestroyed.tag == buttonImage2.sprite.name && itemDestroyed.tag != "Balloon" && itemDestroyed.tag != "Duck") {
                        MoveAndDestroyItem2(itemDestroyed, clipLength);
                    }
                    else {
                        Destroy(itemDestroyed, clipLength);
                    }
                }
                
                itemGridGenerator.setItems(x, i, null);
            }
        }
        else if (currentItem != null && currentItem.tag == "Up Rocket" && currentItem.tag == itemType) {
            RocketScript leftRocketObject = currentItem.GetComponent<RocketScript>();
            GameObject rightRocket = leftRocketObject.GetRightRocket();
            for (int i = 0; i < itemGridGenerator.getAllItems().GetLength(0); i++) {
                GameObject itemDestroyed = itemGridGenerator.getItem(i, y);
                if (itemDestroyed.tag == "Up Rocket" && i != x) {
                    MoveAndExplodeAnotherRocket2(itemDestroyed, x);
                }
                else if (itemDestroyed.tag == "Left Rocket") {
                    MoveAndExplodeAnotherRocket(itemDestroyed, y);
                }
                else if (itemDestroyed.tag == "Up Rocket") {
                    RectTransform leftRocketRectTransform = itemDestroyed.GetComponent<RectTransform>();
                    RectTransform rightRocketRectTransform = rightRocket.GetComponent<RectTransform>();
                    Vector2 leftRocketInitialPosition = leftRocketRectTransform.anchoredPosition;
                    Vector2 rightRocketInitialPosition = rightRocketRectTransform.anchoredPosition;
                    IEnumerator coroutine = MoveRocketUp(leftRocketInitialPosition, rightRocketInitialPosition, leftRocketRectTransform, rightRocketRectTransform, x);
                    StartCoroutine(WaitForCoroutineAndDestroy2(coroutine, itemDestroyed, rightRocket));
                }
                else {
                    audioSource = itemDestroyed.GetComponent<AudioSource>();
                    float clipLength = 0.5f;
                    if (audioSource != null) {
                        audioSource.Play();
                        clipLength = audioSource.clip.length;
                    }
                    
                    Vector2 fallingPosition = itemDestroyed.GetComponent<RectTransform>().anchoredPosition;
                    StartCoroutine(SpawnAndAnimateFallingObjects(fallingPosition, itemDestroyed.tag));
                    if (itemDestroyed.tag == "Balloon" && itemDestroyed.tag == buttonImage1.sprite.name) {
                        if (goal1Count > 0)
                            goal1Count--;
                    }
                    if (itemDestroyed.tag == "Balloon" && itemDestroyed.tag == buttonImage2.sprite.name) {
                        if (goal2Count > 0)
                            goal2Count--;
                    }
                    if (itemDestroyed.tag == buttonImage1.sprite.name && itemDestroyed.tag != "Balloon" && itemDestroyed.tag != "Duck") {
                        MoveAndDestroyItem(itemDestroyed, clipLength);
                    }
                    else if (itemDestroyed.tag == buttonImage2.sprite.name && itemDestroyed.tag != "Balloon" && itemDestroyed.tag != "Duck") {
                        MoveAndDestroyItem2(itemDestroyed, clipLength);
                    }
                    else {
                        Destroy(itemDestroyed, clipLength);
                    }
                }
                
                itemGridGenerator.setItems(i, y, null);
            }
        }
        else if (currentItem != null && currentItem.tag == itemType)
        {
            audioSource = currentItem.GetComponent<AudioSource>();
            float clipLength = 0.5f;
            if (audioSource != null) {
                audioSource.Play();
                clipLength = audioSource.clip.length;
            }
            Vector2 fallingPosition = currentItem.GetComponent<RectTransform>().anchoredPosition;
            StartCoroutine(SpawnAndAnimateFallingObjects(fallingPosition, currentItem.tag));
            if (currentItem.tag == buttonImage1.sprite.name && currentItem.tag != "Balloon" && currentItem.tag != "Duck") {
                MoveAndDestroyItem(currentItem, clipLength);
            }
            else if (currentItem.tag == buttonImage2.sprite.name && currentItem.tag != "Balloon" && currentItem.tag != "Duck") {
                MoveAndDestroyItem2(currentItem, clipLength);
            }
            else {
                Destroy(currentItem, clipLength);
            }
            numberOfItemsDestroyed++;
            itemGridGenerator.setItems(x, y, null);

            DestroyItemAndNeighbors(x - 1, y, itemType);
            DestroyItemAndNeighbors(x + 1, y, itemType);
            DestroyItemAndNeighbors(x, y - 1, itemType);
            DestroyItemAndNeighbors(x, y + 1, itemType);
        }
        else if (currentItem != null && currentItem.tag == "Balloon") {
            audioSource = currentItem.GetComponent<AudioSource>();
            float clipLength = 0.5f;
            if (audioSource != null) {
                audioSource.Play();
                clipLength = audioSource.clip.length;
            }
            if (currentItem.tag == buttonImage1.sprite.name) {
                if (goal1Count > 0)
                    goal1Count--;
            }
            if (currentItem.tag == buttonImage2.sprite.name) {
                if (goal2Count > 0)
                    goal2Count--;
            }
            Destroy(currentItem, clipLength);
            
            itemGridGenerator.setItems(x, y, null);
        }
    }

    private void MoveAndDestroyItem(GameObject obj, float clipLength) {
        RectTransform completedItemRectTransform = obj.GetComponent<RectTransform>();
        Vector2 startingPosition = completedItemRectTransform.anchoredPosition;
        IEnumerator coroutine = MoveCompletedGoals(startingPosition, completedItemRectTransform, goal1);
        StartCoroutine(WaitForCoroutineAndDestroy(coroutine, obj, clipLength));
        if (goal1Count > 0)
            goal1Count--;
    }

    private void MoveAndDestroyItem2(GameObject obj, float clipLength) {
        RectTransform completedItemRectTransform = obj.GetComponent<RectTransform>();
        Vector2 startingPosition = completedItemRectTransform.anchoredPosition;
        IEnumerator coroutine = MoveCompletedGoals(startingPosition, completedItemRectTransform, goal2);
        StartCoroutine(WaitForCoroutineAndDestroy(coroutine, obj, clipLength));
        if (goal2Count > 0)
            goal2Count--;
    }
}