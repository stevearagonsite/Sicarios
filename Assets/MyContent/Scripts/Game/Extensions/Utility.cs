using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public enum Direction {
	Up		= 0,
	Right	= 1,
	Down	= 2,
	Left	= 3
}

public static class Utility {
	public static T Log<T>(T param, string message = "") {
		try {
			Debug.Log(message +  param.ToString());
		}
		catch {
			Debug.Log(message +  "(null)");
		}
		return param;
	}

	public static IEnumerable<T> Generate<T>(T seed, Func<T, T> mutate)
    {
        var accum = seed;
        while (true)
        {
            yield return accum;
            accum = mutate(accum);
        }
    }

	public static IEnumerable<Transform> RecursiveWalker(Transform parent) {
		foreach(Transform child in parent) {
			foreach(Transform grandchild in RecursiveWalker(child))
				yield return grandchild;
			yield return child;
		}
	}

	public static IEnumerable<Tuple<int, int, T>> LazyMatrix<T>(T[,] matrix) {
		for (int i = 0;  i < matrix.GetLength(0); i++)
			for (int j = 0;  j < matrix.GetLength(1); j++)
				yield return Tuple.Create(i, j, matrix[i, j]);
	}

	//											  U  R   D   L
	//											  N  E   S   W
	public static int[] xMoveVector = new int[] { 0, 1,  0, -1 };
	public static int[] yMoveVector = new int[] { 1, 0, -1,  0 };

	public static void KnuthShuffle<T>(List<T> array) {
		for(int i = 0; i<array.Count-1; i++) {
			var j = Random.Range(i, array.Count);
			if(i != j) {
				var temp = array[j];
				array[j] = array[i];
				array[i] = temp;
			}
		}
	}
}
