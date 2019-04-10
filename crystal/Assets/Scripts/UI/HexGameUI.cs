using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour {

	private static HexCell currentCell;
	private static HexUnit selectedUnit;
	public HexGrid grid;

    /// //////////////////////////////////////////
    public static void OnStopMove()
    {
        selectedUnit = null;
        currentCell = null;
    }
	public void SetEditMode (bool toggle) {
		enabled = !toggle;
		grid.ShowUI(!toggle);
		grid.ClearPath();
	}


    /// //////////////////////////////////////////
    private void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButtonDown(0) && null == selectedUnit) {
				DoSelection();
			}
			else if (selectedUnit && selectedUnit.State == HexUnit.AnimationActionState.stand) {
				if (Input.GetMouseButtonDown(0)) {
					DoMove();
				}
				else {
					DoPathfinding(selectedUnit.Speed);
				}
			}
            if (Input.GetMouseButtonDown(1))
            {
                selectedUnit = null;
                currentCell = null;
                grid.ClearPath();
            }
        }
	}
	private void DoSelection () {
		grid.ClearPath();
		UpdateCurrentCell();
		if (currentCell) {
			selectedUnit = currentCell.Unit;
            currentCell.EnableHighlight(Color.blue);
		}
	}
	private void DoPathfinding (int speed=24) {
		if (UpdateCurrentCell()) {
			if (currentCell && selectedUnit.IsValidDestination(currentCell)) {
				grid.FindPath(selectedUnit.Location, currentCell, speed);
			}
			else {
				grid.ClearPath();
			}
		}
	}
	private void DoMove () {
		if (grid.HasPath) {
            //selectedUnit.Location = currentCell;
            selectedUnit.Move();
            selectedUnit.Travel(grid.GetPath());
			grid.ClearPath();
		}
	}
	private bool UpdateCurrentCell () {
		HexCell cell =
			grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != currentCell) {
			currentCell = cell;
			return true;
		}
		return false;
	}
}