using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IPauseEventReceivable {

	void OnPauseBegin();

	void OnPauseEnd();

	void OnResumeBegin();

	void OnResumeEnd();
}
