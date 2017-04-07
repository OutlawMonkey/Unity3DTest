using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathScript : MonoBehaviour {

	public Transform [] path;
	public Color rayColor = Color.white;


	void OnDrawGizmos(){

		Gizmos.color = rayColor;

		Transform [] path_nodes = transform.GetComponentsInChildren <Transform>() ;

		path = new Transform[path_nodes.Length-1];

		for(int j = 1 ; j < path_nodes.Length; j++){
			path[j-1] = path_nodes[j];
		}

		for (int i = 0; i < path.Length; i++){
			
			Vector3 pos = path[i].position;

			if(i>0){
				Vector3 prev = path [i - 1].position;
				Gizmos.DrawLine (prev, pos);
			}

			Gizmos.DrawWireSphere (pos,0.3f);

		}
	}


}
