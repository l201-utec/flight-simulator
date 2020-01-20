using UnityEngine;
using System.Collections;

public class LandGenerator : MonoBehaviour {

	public GameObject landTile; // the current tile nearest the player
	public GameObject newTile; // the tile prefab
	float tileSize = 20000; // the default size of the prefab tile
	float tileGenerationThreshold = 2500; // the component distance from center past which a new tile instantiates
	bool[] tileChecklist; // a checklist to track whether tiles around the landTile have been laid

	enum Axis
	{
		x,
		z,
		both
	}

	/// <summary>
	/// Instantiate the tile checklist.
	/// </summary>
	void Start()
	{
		tileChecklist = new bool[8];
		for (int i=0; i<8; i++) {
			tileChecklist[i] = false;
		}
	}


		
	/// <summary>
	/// Every frame, determine whether to lay a new tile based upon the player's position.
	/// Also, update the landTile object if it is no longer the nearest to the player.
	/// </summary>
	void Update () {

		if (!TilesAreFull ()) {

			// If the player is near enough to a corner, draw a corner tile if it hasn't yet been drawn
			if (PlaneIsFarEnoughAwayFromTileCenter (Axis.x) && PlaneIsFarEnoughAwayFromTileCenter (Axis.z)) {
				GenerateNewTileIfNecessary (Axis.both);
			} else {

				// If the player is near an adjacent tile, draw it if it hasn't yet been drawn
				if (PlaneIsFarEnoughAwayFromTileCenter(Axis.x)) {
					GenerateNewTileIfNecessary (Axis.x);
				}
				if (PlaneIsFarEnoughAwayFromTileCenter(Axis.z)) {
					GenerateNewTileIfNecessary (Axis.z);
				}
			}
		}

		// Check to see if the current landTile is still the closest one to the player. 
		// If not, find the closest tile and set that as landTile.
		SwitchCurrentTileIfNecessary ();

	}

	/// <summary>
	/// Flag a boolean true for each tile around landTile as each is drawn.
	/// </summary>
	/// <param name="currentAxis">The axis currently parralel to the player's facing, x or z. Both if the player is facing diagonally.</param>
	void FlagChecklistTile(Axis currentAxis)
	{
		switch (currentAxis) {
		case Axis.x:
			if (PlaneDirectionFromTileCenter (Axis.x) > 0) {
				tileChecklist [0] = true;
			} else {
				tileChecklist [1] = true;
			}
			break;
		case Axis.z:
			if (PlaneDirectionFromTileCenter (Axis.z) > 0) {
				tileChecklist [2] = true;
			} else {
				tileChecklist [3] = true;
			}
			break;
		case Axis.both:
			if (PlaneDirectionFromTileCenter (Axis.x) > 0) {
				if (PlaneDirectionFromTileCenter (Axis.z) > 0) {
					tileChecklist [4] = true;
				} else {
					tileChecklist [5] = true;
				}

			} else {
				if (PlaneDirectionFromTileCenter (Axis.z) > 0) {
					tileChecklist [6] = true;
				} else {
					tileChecklist [7] = true;
				}
			}
			break;
		}

	}

	/// <summary>
	/// Check whether all the tiles around the landTile are drawn.
	/// </summary>
	/// <returns><c>true</c>, if all tiles in tileChecklist are true, <c>false</c> otherwise.</returns>
	bool TilesAreFull()
	{
		bool retVal = true;
		foreach (bool item in tileChecklist)
			if (!item) 
				retVal = false;
		return retVal;
	}

	/// <summary>
	/// Check whether the plane is far enough away from landTile's center. The tileGenerationThreshold defines that amount.
	/// </summary>
	/// <returns><c>true</c>, if the plane has crossed beyond the tileGenerationThreshold, <c>false</c> otherwise.</returns>
	/// <param name="currentAxis">The axis currently parralel to the player's facing, x or z. Both if the player is facing diagonally.</param>
	bool PlaneIsFarEnoughAwayFromTileCenter(Axis currentAxis)
	{
		return Mathf.Abs (GetPlaneDistanceFromTileCenter(currentAxis)) > tileGenerationThreshold;
	}

	/// <summary>
	/// Check whether the player the has crossed over to a new tile.
	/// </summary>
	/// <returns><c>true</c>, if the player is no longer directly above landTile, <c>false</c> otherwise.</returns>
	/// <param name="currentAxis">The axis currently parralel to the player's facing, x or z. Both if the player is facing diagonally.</param>
	bool PlayerHasCrossedOverToANewTile(Axis currentAxis)
	{
		return false;
	}

	/// <summary>
	/// Generates the new tile ahead of the player if one doesn't exist in that space yet.
	/// </summary>
	/// <param name="currentAxis">The axis currently parralel to the player's facing, x or z. Both if the player is facing diagonally.</param>
	void GenerateNewTileIfNecessary(Axis currentAxis)
	{
		
		if (NewTileDoesNotYetExist (currentAxis)) {

			Vector3 newPosition = new Vector3();

			switch (currentAxis) {
			case Axis.x:
				newPosition = landTile.transform.position + new Vector3 (tileSize * PlaneDirectionFromTileCenter (Axis.x), 0, 0);
				break;
			case Axis.z:
				newPosition = landTile.transform.position + new Vector3 (0, 0, tileSize * PlaneDirectionFromTileCenter (Axis.x));
				break;
			case Axis.both:
				newPosition = landTile.transform.position + 
								new Vector3 (
									tileSize * PlaneDirectionFromTileCenter (Axis.x), 
									0, 
									tileSize * PlaneDirectionFromTileCenter (Axis.z)
								);
				break;
			}

			Instantiate(newTile, newPosition, landTile.transform.rotation);
			FlagChecklistTile (currentAxis);
		}
	}

	/// <summary>
	/// Check whether a tile exists in the spot we want to create one.
	/// </summary>
	/// <returns><c>true</c>, if a tile already occupies the space where we want to create one, <c>false</c> otherwise.</returns>
	/// <param name="currentAxis">The axis currently parralel to the player's facing, x or z. Both if the player is facing diagonally.</param>
	bool NewTileDoesNotYetExist(Axis currentAxis)
	{
		bool retVal = true; // return value assumes there is no tile in our way unless proven otherwise
		GameObject[] tiles = GameObject.FindGameObjectsWithTag("LandTile"); // all tiles currently instantiated in the scene
		float currentTilePosition = 0,		// single-axis landTile position for x or z, double-axis position for x only
					nextTilePosition = 0, 	// single-axis tile position for x or z, double-axis position for x only
					altCurrentPos = 0,		// double-axis landTile position for z
					altNextPos = 0; 		// double-axis tile position for z


		// iterate through all instantiated tiles
		foreach (GameObject tile in tiles) {

			switch (currentAxis) {
			case Axis.x:
				currentTilePosition = landTile.transform.position.x;
				nextTilePosition = tile.transform.position.x;
				break;
			case Axis.z:
				currentTilePosition = landTile.transform.position.z;
				nextTilePosition = tile.transform.position.z;
				break;
			case Axis.both:
				currentTilePosition = landTile.transform.position.x;
				nextTilePosition = tile.transform.position.x;
				altCurrentPos =  landTile.transform.position.z;
				altNextPos = tile.transform.position.z;
				break;
			}

			// if the tile's single-axis position or first double-axis position is
			// located at the coordinates where we want to create a new tile...
			if (nextTilePosition == currentTilePosition + tileSize * PlaneDirectionFromTileCenter(Axis.x)) {

				// if we're dealing with a corner piece (i.e. double-axis both x and z)...
				if (currentAxis == Axis.both) {
					
					// if tile's second double-axis position is
					// located at the coordinates where we want to create a new tile...
					if (altNextPos == altCurrentPos + tileSize * PlaneDirectionFromTileCenter (Axis.z)) {

						// ...then a tile DOES exist where we want to create one, so return false
						retVal = false;
					}
				} else {
					// we're dealing with an adjacent piece (i.e. single-axis x only or z only)
					// on the axis in question, a tile DOES exist where we want to create one, so return false
					retVal = false;
				}

			}
		}
		return retVal;
	}


	/// <summary>
	/// Gets the plane distance from tile center.
	/// </summary>
	/// <returns>The plane distance from tile center.</returns>
	/// <param name="currentAxis">The axis currently parralel to the player's facing, x or z. Both if the player is facing diagonally.</param>
	float GetPlaneDistanceFromTileCenter(Axis currentAxis)
	{
		float planePosition, landTilePosition;

		if (currentAxis == Axis.x) {
			planePosition = transform.position.x;
			landTilePosition = landTile.transform.position.x;
		}
		else {
			planePosition = transform.position.z;
			landTilePosition = landTile.transform.position.z;
		}
		Debug.Log ("Distance: " + (planePosition - landTilePosition));
		return planePosition - landTilePosition;
	}

	/// <summary>
	/// Planes the direction from tile center.
	/// </summary>
	/// <returns>The direction from tile center.</returns>
	/// <param name="currentAxis">The axis currently parralel to the player's facing, x or z. Both if the player is facing diagonally.</param>
	float PlaneDirectionFromTileCenter(Axis currentAxis)
	{
		
		float distance = GetPlaneDistanceFromTileCenter (currentAxis);
		float retVal = 0;
		if (distance > 0)
			retVal = 1;
		else
			retVal = -1;
		return retVal;
	}

	/// <summary>
	/// Switch the current landTile to a new tile if landTile is no longer the closest tile to the player.
	/// </summary>
	void SwitchCurrentTileIfNecessary()
	{
		if (Mathf.Abs(GetPlaneDistanceFromTileCenter (Axis.x)) > tileSize / 2		// if the player is no longer over landTile on x
			|| Mathf.Abs(GetPlaneDistanceFromTileCenter(Axis.z)) > tileSize / 2) 	// if the player is no longer over landTile on z
		{
			SwitchCurrentTile();
		}
			
	}

	/// <summary>
	/// Reassign landTile to be the closest tile to the player.
	/// </summary>
	void SwitchCurrentTile()
	{
		// all currently instantiated tiles
		GameObject[] tiles = GameObject.FindGameObjectsWithTag("LandTile");

		// iterate through all tiles
		foreach (GameObject tile in tiles) {

			Debug.Log ("Distance between current tile and others: " + (tile.transform.position - landTile.transform.position).magnitude);
			// if the distance between the player and a given tile is less than that of the player and landTile
			if (Mathf.Abs((transform.position - tile.transform.position).magnitude) < Mathf.Abs((transform.position - landTile.transform.position).magnitude)) {

				// Reassign landTile to be the tile that is nearer the player.
				landTile = tile;
			}
		}
	}

}
