using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HexMapEditor : MonoBehaviour
{

    public Color[] colors;

    public HexGrid hexGrid;

    int activeElevation;
    int activeWaterLevel;

    Color activeColor;

    int brushSize;
    int activeUrbanLevel, activeFarmLevel, activePlantLevel;

    bool applyColor = false;
    bool applyElevation = false;
    bool applyWaterLevel = false;
    bool applyUrbanLevel, applyFarmLevel, applyPlantLevel;

    enum OptionalToggle
    {
        Ignore, Yes, No
    }

    OptionalToggle riverMode, roadMode, walledMode;

    bool isDrag;
    HexDirection dragDirection;
    HexCell previousCell;

    public void SelectColor(int index)
    {
        applyColor = index >= 0;
        if (applyColor) {
            activeColor = colors[index];
        }
    }
    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }
    public void SetElevation(Slider elevation)
    {
        activeElevation = (int)elevation.value;
    }
    public void SetApplyWaterLevel(bool toggle)
    {
        applyWaterLevel = toggle;
    }
    public void SetWaterLevel(Slider level)
    {
        activeWaterLevel = (int)level.value;
    }
    public void SetBrushSize(Slider size)
    {
        brushSize = (int)size.value;
    }
    public void SetRiverMode(int mode)
    {
        riverMode = (OptionalToggle)mode;
    }
    public void SetRoadMode(int mode)
    {
        roadMode = (OptionalToggle)mode;
    }
    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }
    void Awake()
    {
        SelectColor(0);
    }
    void Update()
    {
        if (
            Input.GetMouseButton(0) &&
            !EventSystem.current.IsPointerOverGameObject()
        ) {
            HandleInput();
        }
        else {
            previousCell = null;
        }
    }
    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            HexCell currentCell = hexGrid.GetCell(hit.point);
            if (previousCell && previousCell != currentCell) {
                ValidateDrag(currentCell);
            }
            else {
                isDrag = false;
            }
            EditCells(currentCell);
            previousCell = currentCell;
        }
        else {
            previousCell = null;
        }
    }
    void ValidateDrag(HexCell currentCell)
    {
        for (
            dragDirection = HexDirection.NE;
            dragDirection <= HexDirection.NW;
            dragDirection++
        ) {
            if (previousCell.GetNeighbor(dragDirection) == currentCell) {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
    }
    void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++) {
            for (int x = centerX - r; x <= centerX + brushSize; x++) {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++) {
            for (int x = centerX - brushSize; x <= centerX + r; x++) {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }
    void EditCell(HexCell cell)
    {
        if (cell) {
            if (applyColor) {
                cell.Color = activeColor;
            }
            if (applyElevation) {
                cell.Elevation = activeElevation;
            }
            if (applyWaterLevel) {
                cell.WaterLevel = activeWaterLevel;
            }
            if (applyUrbanLevel) {
                cell.UrbanLevel = activeUrbanLevel;
            }
            if (applyFarmLevel) {
                cell.FarmLevel = activeFarmLevel;
            }
            if (applyPlantLevel) {
                cell.PlantLevel = activePlantLevel;
            }
            if (riverMode == OptionalToggle.No) {
                cell.RemoveRiver();
            }
            if (roadMode == OptionalToggle.No) {
                cell.RemoveRoads();
            }
            if (walledMode != OptionalToggle.Ignore) {
                cell.Walled = walledMode == OptionalToggle.Yes;
            }
            if (isDrag) {
                HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                if (otherCell) {
                    if (riverMode == OptionalToggle.Yes) {
                        otherCell.SetOutgoingRiver(dragDirection);
                    }
                    if (roadMode == OptionalToggle.Yes) {
                        otherCell.AddRoad(dragDirection);
                    }
                }
            }
        }
    }
    public void SetApplyUrbanLevel(bool toggle)
    {
        applyUrbanLevel = toggle;
    }
    public void SetUrbanLevel(Slider level)
    {
        activeUrbanLevel = (int)level.value;
    }
    public void SetApplyFarmLevel(bool toggle)
    {
        applyFarmLevel = toggle;
    }
    public void SetFarmLevel(Slider level)
    {
        activeFarmLevel = (int)level.value;
    }
    public void SetApplyPlantLevel(bool toggle)
    {
        applyPlantLevel = toggle;
    }
    public void SetPlantLevel(Slider level)
    {
        activePlantLevel = (int)level.value;
    }
    public void SetWalledMode(int mode)
    {
        walledMode = (OptionalToggle)mode;
    }
}