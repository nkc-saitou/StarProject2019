using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IStageMoveEvent {

	float GetPosition();

	void OnExecute(StageSelectController controller, bool forward, bool warp);

}
