using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGridGenerator : MonoBehaviour
{
    [SerializeField] int rows;
    [SerializeField] int columns;
    [SerializeField] GameObject[] candyPrefabs;
    private GameObject[,] items;
    private Vector2[,] itemPositions;
    private RectTransform rectTransform;
    private RectTransform itemsRectTransform;
    [SerializeField] float spacing;
    float cellSize = 0;
    private AudioSource audioSource;
    public delegate void ItemClickHandler(GameObject item);
    public event ItemClickHandler OnItemClick;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        itemsRectTransform = transform.Find("Items").GetComponent<RectTransform>();
        CreateItemGrid();
    }

    public RectTransform getItemsRectTransform()
    {
        return itemsRectTransform;
    }

    public float getCellSize()
    {
        return cellSize;
    }

    public GameObject getCandyPrefab(int index) {
        return candyPrefabs[index];
    }

    private void CreateItemGrid()
    {

        cellSize = (rectTransform.rect.width - spacing * (columns - 1)) / columns;
        itemsRectTransform.sizeDelta = new Vector2(rectTransform.rect.width, cellSize * rows + spacing * (rows - 1));

        GameObject randomItem = null;
        items = new GameObject[rows, columns];
        itemPositions = new Vector2[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {

                float xPos = j * (cellSize + spacing);
                float yPos = -i * (cellSize + spacing);
                int randomIndex;
                if (i == rows - 1) 
                    randomIndex = Random.Range(0, candyPrefabs.Length - 6);
                else
                    randomIndex = Random.Range(0, candyPrefabs.Length - 5);
                randomItem = candyPrefabs[randomIndex];

                GameObject item = Instantiate(randomItem, itemsRectTransform);

                RectTransform candyRectTransform = item.GetComponent<RectTransform>();

                candyRectTransform.sizeDelta = new Vector2(cellSize, cellSize);
                candyRectTransform.anchoredPosition = new Vector2(xPos - ((columns - 1) * spacing / 2.0f + ((columns / 2.0f) - 0.5f) * cellSize), yPos + ((rows - 1) * spacing / 2.0f + ((rows / 2.0f) - 0.5f) * cellSize));
                itemPositions[i, j] = candyRectTransform.anchoredPosition;
                Button itemButton = item.GetComponent<Button>();
                if (itemButton != null)
                {
                    itemButton.onClick.AddListener(() => OnItemClick?.Invoke(item));
                }
                items[i, j] = item;

            }
        }
    }

    IEnumerator AnimateItemMovement(RectTransform itemRectTransform, Vector2 initialPosition, Vector2 finalPosition, float duration, GameObject obj)
    {
        bool check = false;
        RectTransform rightRocketRectTransform = null;
        GameObject rightRocketReference = null;
        if (obj != null && (obj.tag == "Left Rocket" || obj.tag == "Up Rocket")) {
            check = true;
            RocketScript leftRocketObject = obj.GetComponent<RocketScript>();
            rightRocketReference = leftRocketObject.GetRightRocket(); 
        }
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = Mathf.Clamp01(elapsedTime / duration);
            if (itemRectTransform != null)
                if (check) {
                    if (obj.tag == "Left Rocket") {
                        itemRectTransform.anchoredPosition = Vector2.Lerp(initialPosition - new Vector2(cellSize * 0.25f, 0), finalPosition - new Vector2(cellSize * 0.25f, 0), t);
                        rightRocketRectTransform = rightRocketReference.GetComponent<RectTransform>();
                        rightRocketRectTransform.anchoredPosition = Vector2.Lerp(initialPosition + new Vector2(cellSize * 0.25f, 0), finalPosition + new Vector2(cellSize * 0.25f, 0), t);
                    }
                    else {
                        itemRectTransform.anchoredPosition = Vector2.Lerp(initialPosition + new Vector2(0, cellSize * 0.25f), finalPosition + new Vector2(0, cellSize * 0.25f), t);
                        rightRocketRectTransform = rightRocketReference.GetComponent<RectTransform>();
                        rightRocketRectTransform.anchoredPosition = Vector2.Lerp(initialPosition - new Vector2(0, cellSize * 0.25f), finalPosition - new Vector2(0, cellSize * 0.25f), t);
                    }
                }
                else {
                    itemRectTransform.anchoredPosition = Vector2.Lerp(initialPosition, finalPosition, t);
                }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (itemRectTransform != null) {
            if (check && rightRocketRectTransform != null) {
                if (obj.tag == "Left Rocket") {
                    itemRectTransform.anchoredPosition = finalPosition - new Vector2(cellSize * 0.25f, 0);
                    rightRocketRectTransform.anchoredPosition = finalPosition + new Vector2(cellSize * 0.25f, 0);
                }
                else {
                    itemRectTransform.anchoredPosition = finalPosition + new Vector2(0, cellSize * 0.25f);
                    rightRocketRectTransform.anchoredPosition = finalPosition - new Vector2(0, cellSize * 0.25f);
                }
            }
            else {
                itemRectTransform.anchoredPosition = finalPosition;
            }
        }
    }

    public bool checkDuckAtBottom(int col) {
        if (items[items.GetLength(0) - 1, col].tag == "Duck")
        {
            audioSource = items[items.GetLength(0) - 1, col].GetComponent<AudioSource>();
            audioSource.Play();
            float clipLength = audioSource.clip.length;
            Destroy(items[items.GetLength(0) - 1, col], clipLength);
            items[items.GetLength(0) - 1, col] = null;
            return true;
        }
        else {
            return false;
        }
    }

    public void fillItemGrid(int xIndex, int yIndex, bool flag)
    {
        if (flag) {
            GameObject leftRocketItem = null;
            GameObject rightRocketItem = null;
            int selectedRocketDirection = Random.Range(0, 2);
            if (selectedRocketDirection == 0) {
                leftRocketItem = candyPrefabs[7];
                rightRocketItem = candyPrefabs[8];
            }
            else if (selectedRocketDirection == 1) {
                leftRocketItem = candyPrefabs[10];
                rightRocketItem = candyPrefabs[11];
            }
            
            GameObject leftRocket = Instantiate(leftRocketItem, itemsRectTransform);
            RocketScript leftRocketScript = leftRocket.GetComponent<RocketScript>();
            RectTransform leftRocketRectTransform = leftRocket.GetComponent<RectTransform>();
            GameObject rightRocket = Instantiate(rightRocketItem, itemsRectTransform);
            RectTransform rightRocketRectTransform = rightRocket.GetComponent<RectTransform>();

            if (selectedRocketDirection == 0) {
                leftRocketScript.SetDirection("left");
                leftRocketRectTransform.sizeDelta = new Vector2(cellSize * 0.5f, cellSize * 0.5f);
                leftRocketRectTransform.anchoredPosition = itemPositions[xIndex, yIndex] - new Vector2(cellSize * 0.25f, 0);
                rightRocketRectTransform.sizeDelta = new Vector2(cellSize * 0.5f, cellSize * 0.5f);
                rightRocketRectTransform.anchoredPosition = itemPositions[xIndex, yIndex] + new Vector2(cellSize * 0.25f, 0);
            }
            else if (selectedRocketDirection == 1) {
                leftRocketScript.SetDirection("up");
                leftRocketRectTransform.sizeDelta = new Vector2(cellSize * 0.5f, cellSize * 0.5f);
                leftRocketRectTransform.anchoredPosition = itemPositions[xIndex, yIndex] + new Vector2(0, cellSize * 0.25f);
                rightRocketRectTransform.sizeDelta = new Vector2(cellSize * 0.5f, cellSize * 0.5f);
                rightRocketRectTransform.anchoredPosition = itemPositions[xIndex, yIndex] - new Vector2(0, cellSize * 0.25f);
            }
        
            leftRocketScript.SetRightRocket(rightRocket);

            Button itemButton1 = leftRocket.GetComponent<Button>();
            if (itemButton1 != null)
            {
                itemButton1.onClick.AddListener(() => OnItemClick?.Invoke(leftRocket));
            }
            items[xIndex, yIndex] = leftRocket;
        }
        for (int col = 0; col < items.GetLength(1); col++)
        {
            int rowToFill = 0;
            int rowUsedToFill = -1;
            bool check = false;
            for (int row = items.GetLength(0) - 1; row >= 0; row--)
            {
                if (items[row, col] == null && !check)
                {
                    rowToFill = row;
                    check = true;
                    continue;
                }
                if (items[row, col] != null && check)
                {
                    rowUsedToFill = row;
                    Vector2 initialPosition = itemPositions[rowUsedToFill, col];
                    Vector2 finalPosition = itemPositions[rowToFill, col];
                    RectTransform candyRectTransform = items[row, col].GetComponent<RectTransform>();
                    float animationDuration = 0.5f;
                    StartCoroutine(AnimateItemMovement(candyRectTransform, initialPosition, finalPosition, animationDuration, items[row, col]));
                    items[rowToFill, col] = items[row, col];
                    items[row, col] = null;
                    bool bottomDuck = checkDuckAtBottom(col);
                    if (bottomDuck) {
                        row = items.GetLength(0) - 1;
                        rowToFill = row;
                        check = true;
                    }
                    else {
                        row = rowToFill;
                        check = false;
                        rowToFill = 0;
                    }
                }
            }
            for (int j = items.GetLength(0) - 1; j >= 0; j--)
            {
                if (items[j, col] == null)
                {
                    int randomIndex = Random.Range(0, candyPrefabs.Length - 5);
                    GameObject randomItem = candyPrefabs[randomIndex];

                    GameObject item = Instantiate(randomItem, itemsRectTransform);
                    RectTransform candyRectTransform = item.GetComponent<RectTransform>();

                    candyRectTransform.sizeDelta = new Vector2(cellSize, cellSize);

                    Vector2 initialPosition = itemPositions[j, col] + new Vector2(0, cellSize);
                    Vector2 finalPosition = itemPositions[j, col];

                    float animationDuration = 0.5f;
                    StartCoroutine(AnimateItemMovement(candyRectTransform, initialPosition, finalPosition, animationDuration, null));
                    Button itemButton = item.GetComponent<Button>();
                    if (itemButton != null)
                    {
                        itemButton.onClick.AddListener(() => OnItemClick?.Invoke(item));
                    }
                    items[j, col] = item;
                }
            }
        }
    }

    public GameObject[,] getAllItems()
    {
        return items;
    }

    public void setItems(int row, int col, GameObject obj)
    {
        if (row >= 0 && row < items.GetLength(0) && col >= 0 && col < items.GetLength(1))
        {
            this.items[row, col] = obj;
        }
        else
        {
            return;
        }
    }

    public GameObject getItem(int x, int y)
    {
        if (x >= 0 && x < items.GetLength(0) && y >= 0 && y < items.GetLength(1))
        {
            return items[x, y];
        }
        else
        {
            return null;
        }
    }

    public Vector2Int getItemIndex(GameObject obj)
    {
        int xIndex = 0;
        int yIndex = 0;
        bool check = false;
        for (int i = 0; i < items.GetLength(0); i++)
        {
            for (int j = 0; j < items.GetLength(1); j++)
            {
                if (items[i, j] == obj)
                {
                    xIndex = i;
                    yIndex = j;
                    check = true;
                    break;
                }
            }
            if (check)
                break;
        }
        return new Vector2Int(xIndex, yIndex);
    }
}
