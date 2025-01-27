using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Game_Manager : MonoBehaviour
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZAEIOUY";
    private List<string> Main_Dictionary = new List<string>();
    private readonly System.Random rnd = new System.Random();
    [Header("Dictionary Asset")]
    public TextAsset Dictionary_Text_Asset = null;
    [Header("Object References")]
    public GameObject Letter_Tile_Prefab = null;
    public Transform Letter_Grid = null;
    public Text Current_Word_Text = null;
    [Header("Bounds")]
    public RectTransform Game_Bounds = null;
    private Vector2 Grid_Pixel_Min = Vector2.zero;
    private Vector2 Grid_Pixel_Max = Vector2.zero;
    [Header("Buttons")]
    public RectTransform Quit_Button = null;
    private Vector2 Quit_Pixel_Min = Vector2.zero;
    private Vector2 Quit_Pixel_Max = Vector2.zero;
    public RectTransform Refresh_Button = null;
    private Vector2 Refresh_Pixel_Min = Vector2.zero;
    private Vector2 Refresh_Pixel_Max = Vector2.zero;

    public List<Text> History_Slots = new List<Text>();
    private List<string> History = new List<string>();

    private List<Letter_Tile> Letter_Tiles = new List<Letter_Tile>();
    private List<Letter_Tile> Selected_Letter_Tiles = new List<Letter_Tile>();
    private string Current_Word = "";
    void Start()
    {
        Grid_Pixel_Max = Camera.main.WorldToScreenPoint(Game_Bounds.TransformPoint(Game_Bounds.rect.max));
        Grid_Pixel_Min = Camera.main.WorldToScreenPoint(Game_Bounds.TransformPoint(Game_Bounds.rect.min));
        Refresh_Pixel_Max = Camera.main.WorldToScreenPoint(Refresh_Button.TransformPoint(Refresh_Button.rect.max));
        Refresh_Pixel_Min = Camera.main.WorldToScreenPoint(Refresh_Button.TransformPoint(Refresh_Button.rect.min));
        Quit_Pixel_Max = Camera.main.WorldToScreenPoint(Quit_Button.TransformPoint(Quit_Button.rect.max));
        Quit_Pixel_Min = Camera.main.WorldToScreenPoint(Quit_Button.TransformPoint(Quit_Button.rect.min));

        foreach (string Word in Dictionary_Text_Asset.text.Split('\n'))
        {
            Main_Dictionary.Add(Word.Replace("\n", "").Replace("\r", "").Replace(" ", "").ToUpper());
        }

        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                Letter_Tile New_Tile = new Letter_Tile();
                New_Tile.Tile_Position = new Vector2Int(x, y);
                GameObject Letter_Tile = Instantiate(Letter_Tile_Prefab, Letter_Grid);
                New_Tile.Letter = Alphabet.Substring(rnd.Next(0, 25), 1);
                New_Tile.Text_Reference = Letter_Tile.transform.GetChild(0).GetChild(0).GetComponent<Text>();
                New_Tile.Text_Reference.text = New_Tile.Letter;
                New_Tile.Image_Reference = Letter_Tile.transform.GetChild(0).GetComponent<Image>();
                Letter_Tile.name = $"Letter Tile ({x},{y})";
                RectTransform Letter_Tile_Rect_Transform = Letter_Tile.GetComponent<RectTransform>();
                Letter_Tile_Rect_Transform.anchorMin = new Vector2(x / 5f, y / 5f);
                Letter_Tile_Rect_Transform.anchorMax = new Vector2((x + 1) / 5f, (y + 1) / 5f);
                Letter_Tile_Rect_Transform.anchoredPosition = new Vector2(0, 0);
                Letter_Tile_Rect_Transform.sizeDelta = Vector2.one;
                New_Tile.Pixel_Min = Camera.main.WorldToScreenPoint(Letter_Tile_Rect_Transform.TransformPoint(Letter_Tile_Rect_Transform.rect.min));
                New_Tile.Pixel_Max = Camera.main.WorldToScreenPoint(Letter_Tile_Rect_Transform.TransformPoint(Letter_Tile_Rect_Transform.rect.max));
                New_Tile.Pixel_Position = Camera.main.WorldToScreenPoint(Letter_Tile.transform.position);
                Letter_Tiles.Add(New_Tile);
            }
        }
    }
    void Update()
    {
        if (true == false && Input.mousePresent)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Selected_Letter_Tiles = new List<Letter_Tile>();
                if (Input.mousePosition.x <= Grid_Pixel_Max.x && Input.mousePosition.x >= Grid_Pixel_Min.x && Input.mousePosition.y <= Grid_Pixel_Max.y && Input.mousePosition.y >= Grid_Pixel_Min.y)
                {
                    foreach (Letter_Tile LT in Letter_Tiles)
                    {
                        if (Input.mousePosition.x >= LT.Pixel_Min.x && Input.mousePosition.x <= LT.Pixel_Max.x && Input.mousePosition.y >= LT.Pixel_Min.y && Input.mousePosition.y <= LT.Pixel_Max.y)
                        {
                            Selected_Letter_Tiles.Add(LT);
                            break;
                        }
                    }
                }
                else if (Input.mousePosition.x <= Quit_Pixel_Max.x && Input.mousePosition.x >= Quit_Pixel_Min.x && Input.mousePosition.y <= Quit_Pixel_Max.y && Input.mousePosition.y >= Quit_Pixel_Min.y)
                {
                    Application.Quit();
                }
                else if (Input.mousePosition.x <= Refresh_Pixel_Max.x && Input.mousePosition.x >= Refresh_Pixel_Min.x && Input.mousePosition.y <= Refresh_Pixel_Max.y && Input.mousePosition.y >= Refresh_Pixel_Min.y)
                {
                    Selected_Letter_Tiles = new List<Letter_Tile>();
                    History = new List<string>();
                    foreach (Letter_Tile LT in Letter_Tiles)
                    {
                        LT.Letter = Alphabet.Substring(rnd.Next(0, 25), 1);
                        LT.Text_Reference.text = LT.Letter;
                    }
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (Selected_Letter_Tiles.Count > 0)
                {
                    Letter_Tile Hit_Tile = null;
                    foreach (Letter_Tile LT in Letter_Tiles)
                    {
                        if (Input.mousePosition.x >= LT.Pixel_Min.x && Input.mousePosition.x <= LT.Pixel_Max.x && Input.mousePosition.y >= LT.Pixel_Min.y && Input.mousePosition.y <= LT.Pixel_Max.y)
                        {
                            Hit_Tile = LT;
                            break;
                        }
                    }
                    if (Hit_Tile == null || Hit_Tile.Tile_Position != Selected_Letter_Tiles[Selected_Letter_Tiles.Count - 1].Tile_Position)
                    {
                        float Move_Direction = Rotation_Helper.Deg_Clamp(Rotation_Helper.Vector_To_Deg((Vector2)Input.mousePosition - Selected_Letter_Tiles[Selected_Letter_Tiles.Count - 1].Pixel_Position) + 22.5f);
                        Vector2Int Move_Direction_Int = Vector2Int.zero;
                        if (Move_Direction < 45)
                        {
                            Move_Direction_Int = new Vector2Int(0, 1);
                        }
                        else if (Move_Direction >= 45 && Move_Direction < 90)
                        {
                            Move_Direction_Int = new Vector2Int(1, 1);
                        }
                        else if (Move_Direction >= 90 && Move_Direction < 135)
                        {
                            Move_Direction_Int = new Vector2Int(1, 0);
                        }
                        else if (Move_Direction >= 135 && Move_Direction < 180)
                        {
                            Move_Direction_Int = new Vector2Int(1, -1);
                        }
                        else if (Move_Direction >= 180 && Move_Direction < 225)
                        {
                            Move_Direction_Int = new Vector2Int(0, -1);
                        }
                        else if (Move_Direction >= 225 && Move_Direction < 270)
                        {
                            Move_Direction_Int = new Vector2Int(-1, -1);
                        }
                        else if (Move_Direction >= 270 && Move_Direction < 315)
                        {
                            Move_Direction_Int = new Vector2Int(-1, 0);
                        }
                        else if (Move_Direction >= 315)
                        {
                            Move_Direction_Int = new Vector2Int(-1, 1);
                        }
                        if (Move_Direction_Int != Vector2Int.zero)
                        {
                            foreach (Letter_Tile LT in Letter_Tiles)
                            {
                                if (LT.Tile_Position == Selected_Letter_Tiles[Selected_Letter_Tiles.Count - 1].Tile_Position + Move_Direction_Int)
                                {
                                    if (Selected_Letter_Tiles.Count >= 2 && Selected_Letter_Tiles[Selected_Letter_Tiles.Count - 2].Tile_Position == LT.Tile_Position)
                                    {
                                        Selected_Letter_Tiles.RemoveAt(Selected_Letter_Tiles.Count - 1);
                                        break;
                                    }
                                    else
                                    {
                                        bool Overlap = false;
                                        foreach (Letter_Tile SLT in Selected_Letter_Tiles)
                                        {
                                            if (SLT.Tile_Position == LT.Tile_Position)
                                            {
                                                Overlap = true;
                                            }
                                        }
                                        if (Selected_Letter_Tiles.Count < 7 && !Overlap)
                                        {
                                            Selected_Letter_Tiles.Add(LT);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Selected_Letter_Tiles = new List<Letter_Tile>();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (Selected_Letter_Tiles.Count > 0)
                {
                    Current_Word = "";
                    foreach (Letter_Tile LT in Selected_Letter_Tiles)
                    {
                        Current_Word += LT.Letter.ToUpper();
                    }
                    foreach (string word in Main_Dictionary)
                    {
                        if (word == Current_Word)
                        {
                            while (History.Contains(Current_Word))
                            {
                                History.Remove(Current_Word);
                            }
                            History.Insert(0, Current_Word);
                            break;
                        }
                    }
                }
                Current_Word = "";
                Selected_Letter_Tiles = new List<Letter_Tile>();
            }
        }
        else
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Selected_Letter_Tiles = new List<Letter_Tile>();
                    if (Input.GetTouch(0).position.x <= Grid_Pixel_Max.x && Input.GetTouch(0).position.x >= Grid_Pixel_Min.x && Input.GetTouch(0).position.y <= Grid_Pixel_Max.y && Input.GetTouch(0).position.y >= Grid_Pixel_Min.y)
                    {
                        foreach (Letter_Tile LT in Letter_Tiles)
                        {
                            if (Input.GetTouch(0).position.x >= LT.Pixel_Min.x && Input.GetTouch(0).position.x <= LT.Pixel_Max.x && Input.GetTouch(0).position.y >= LT.Pixel_Min.y && Input.GetTouch(0).position.y <= LT.Pixel_Max.y)
                            {
                                Selected_Letter_Tiles.Add(LT);
                                break;
                            }
                        }
                    }
                    else if (Input.GetTouch(0).position.x <= Quit_Pixel_Max.x && Input.GetTouch(0).position.x >= Quit_Pixel_Min.x && Input.GetTouch(0).position.y <= Quit_Pixel_Max.y && Input.GetTouch(0).position.y >= Quit_Pixel_Min.y)
                    {
                        Application.Quit();
                    }
                    else if (Input.GetTouch(0).position.x <= Refresh_Pixel_Max.x && Input.GetTouch(0).position.x >= Refresh_Pixel_Min.x && Input.GetTouch(0).position.y <= Refresh_Pixel_Max.y && Input.GetTouch(0).position.y >= Refresh_Pixel_Min.y)
                    {
                        Selected_Letter_Tiles = new List<Letter_Tile>();
                        History = new List<string>();
                        foreach (Letter_Tile LT in Letter_Tiles)
                        {
                            LT.Letter = Alphabet.Substring(rnd.Next(0, 25), 1);
                            LT.Text_Reference.text = LT.Letter;
                        }
                    }
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary)
                {
                    if (Selected_Letter_Tiles.Count > 0)
                    {
                        Letter_Tile Hit_Tile = null;
                        foreach (Letter_Tile LT in Letter_Tiles)
                        {
                            if (Input.GetTouch(0).position.x >= LT.Pixel_Min.x && Input.GetTouch(0).position.x <= LT.Pixel_Max.x && Input.GetTouch(0).position.y >= LT.Pixel_Min.y && Input.GetTouch(0).position.y <= LT.Pixel_Max.y)
                            {
                                Hit_Tile = LT;
                                break;
                            }
                        }
                        if (Hit_Tile == null || Hit_Tile.Tile_Position != Selected_Letter_Tiles[Selected_Letter_Tiles.Count - 1].Tile_Position)
                        {
                            float Move_Direction = Rotation_Helper.Deg_Clamp(Rotation_Helper.Vector_To_Deg((Vector2)Input.GetTouch(0).position - Selected_Letter_Tiles[Selected_Letter_Tiles.Count - 1].Pixel_Position) + 22.5f);
                            Vector2Int Move_Direction_Int = Vector2Int.zero;
                            if (Move_Direction < 45)
                            {
                                Move_Direction_Int = new Vector2Int(0, 1);
                            }
                            else if (Move_Direction >= 45 && Move_Direction < 90)
                            {
                                Move_Direction_Int = new Vector2Int(1, 1);
                            }
                            else if (Move_Direction >= 90 && Move_Direction < 135)
                            {
                                Move_Direction_Int = new Vector2Int(1, 0);
                            }
                            else if (Move_Direction >= 135 && Move_Direction < 180)
                            {
                                Move_Direction_Int = new Vector2Int(1, -1);
                            }
                            else if (Move_Direction >= 180 && Move_Direction < 225)
                            {
                                Move_Direction_Int = new Vector2Int(0, -1);
                            }
                            else if (Move_Direction >= 225 && Move_Direction < 270)
                            {
                                Move_Direction_Int = new Vector2Int(-1, -1);
                            }
                            else if (Move_Direction >= 270 && Move_Direction < 315)
                            {
                                Move_Direction_Int = new Vector2Int(-1, 0);
                            }
                            else if (Move_Direction >= 315)
                            {
                                Move_Direction_Int = new Vector2Int(-1, 1);
                            }
                            if (Move_Direction_Int != Vector2Int.zero)
                            {
                                foreach (Letter_Tile LT in Letter_Tiles)
                                {
                                    if (LT.Tile_Position == Selected_Letter_Tiles[Selected_Letter_Tiles.Count - 1].Tile_Position + Move_Direction_Int)
                                    {
                                        if (Selected_Letter_Tiles.Count >= 2 && Selected_Letter_Tiles[Selected_Letter_Tiles.Count - 2].Tile_Position == LT.Tile_Position)
                                        {
                                            Selected_Letter_Tiles.RemoveAt(Selected_Letter_Tiles.Count - 1);
                                            break;
                                        }
                                        else
                                        {
                                            bool Overlap = false;
                                            foreach (Letter_Tile SLT in Selected_Letter_Tiles)
                                            {
                                                if (SLT.Tile_Position == LT.Tile_Position)
                                                {
                                                    Overlap = true;
                                                }
                                            }
                                            if (Selected_Letter_Tiles.Count < 7 && !Overlap)
                                            {
                                                Selected_Letter_Tiles.Add(LT);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Selected_Letter_Tiles = new List<Letter_Tile>();
                    }
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
                {
                    if (Selected_Letter_Tiles.Count > 0)
                    {
                        Current_Word = "";
                        foreach (Letter_Tile LT in Selected_Letter_Tiles)
                        {
                            Current_Word += LT.Letter.ToUpper();
                        }
                        foreach (string word in Main_Dictionary)
                        {
                            if (word == Current_Word)
                            {
                                while (History.Contains(Current_Word))
                                {
                                    History.Remove(Current_Word);
                                }
                                History.Insert(0, Current_Word);
                                break;
                            }
                        }
                    }
                    Current_Word = "";
                    Selected_Letter_Tiles = new List<Letter_Tile>();
                }
            }
        }

        foreach (Letter_Tile LT in Letter_Tiles)
        {
            LT.Image_Reference.color = Color.white;
        }
        Current_Word = "";
        foreach (Letter_Tile LT in Selected_Letter_Tiles)
        {
            LT.Image_Reference.color = Color.cyan;
            Current_Word += LT.Letter;
        }

        Current_Word = Current_Word.ToUpper();
        Current_Word_Text.text = Current_Word;
        Current_Word_Text.color = Color.red;
        foreach (string word in Main_Dictionary)
        {
            if (word == Current_Word)
            {
                Current_Word_Text.color = Color.black;
                break;
            }
        }

        for (int i = 0; i < History_Slots.Count; i++)
        {
            if (i < History.Count)
            {
                History_Slots[i].text = History[i];
            }
            else
            {
                History_Slots[i].text = "";
            }
        }
    }
}
public class Letter_Tile
{
    public Vector2Int Tile_Position = Vector2Int.zero;
    public Vector2 Pixel_Position = Vector2.zero;
    public Vector2 Pixel_Min = Vector2.zero;
    public Vector2 Pixel_Max = Vector2.zero;
    public string Letter = "A";
    public Text Text_Reference = null;
    public Image Image_Reference = null;
}