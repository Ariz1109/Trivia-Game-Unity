using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TriviaGameLoop : MonoBehaviour
{

	//This is the stuct that sets up what our question values are going to be.
	public struct Question
	{
		public string questionText;
		public string[] answers;
		public int correctAnswerIndex;

		public Question(string questionText, string[] answers, int correctAnswerIndex)
		{
			this.questionText = questionText;
			this.answers = answers;
			this.correctAnswerIndex = correctAnswerIndex;
		}
	}

	private Question currentQuestion = new Question("What is your favorite color?", new string[] { "blue", "red", "yellow", "white", "black" }, 0);
	public Button[] answerButtons;
	public Text questionText;

	private Question[] questions = new Question[10];
	private int currentQuestionIndex;
	private int[] questionNumbersChoosen = new int[5];
	private int questionsFinished;

	public GameObject[] TriviaPanels;
	public GameObject finalResultsPanel;
	public Text resultsText;
	private int numberOfCorrectAnswers;
	private bool allowSelection = true;
	public GameObject feedbackText;

	public AudioSource correctSound;
	public AudioSource incorrectSound;

	// Use this for initialization
	void Start()
	{


		questions[0] = new Question("What is the capital of Spain?", new string[] { "Topeka", "Amsterdam", "Madrid", "London", "Toledo" }, 2);
		questions[1] = new Question("Who was the second US president?", new string[] { "Thomas Jefferson", "John Adams", "Bill Clinton", "George Washington", "Abraham Lincon" }, 1);
		questions[2] = new Question("What is the second planet in our solor system?", new string[] { "Mercury", "Earth", "Saturn", "Venus", "Pluto" }, 3);
		questions[3] = new Question("What is the largest continent?", new string[] { "Africa", "North America", "Asia", "Europe", "Austalia" }, 2);
		questions[4] = new Question("What US state has the hightest population?", new string[] { "California", "Florida", "Texas", "New York", "North Carolina" }, 0);
		questions[5] = new Question("A Platypus is a _____", new string[] { "Bird", "Reptile", "Insect", "Amphibian", "Mammal" }, 4);
		questions[6] = new Question("What is the boiling tempurature in fahrenheit?", new string[] { "100 degrees", "190 degrees", "300 degrees", "312 degreesn", "212 degrees" }, 4);
		questions[7] = new Question("How many degrees are in a circle?", new string[] { "360", "180", "640", "16", "270" }, 0);
		questions[8] = new Question("What is a name for a group of crows?", new string[] { "A bloat", "A herd", "A pack", "A murder", "A team" }, 3);
		questions[9] = new Question("Who created the painting starry night?", new string[] { "Pablo Picasso", "Vincent van Gogh", "Andy Warhol", "Leonardo da Vinci", "Frida Kahlo" }, 1);

		chooseQuestions();
		assignQuestion(questionNumbersChoosen[0]);

	}


	// Update is called once per frame
	void Update()
	{
		quitGame();
	}

	//Settting up the interface to show a question
	void assignQuestion(int questionNum)
	{
		currentQuestion = questions[questionNum];

		// Mezclar las respuestas antes de asignarlas
		ShuffleAnswers(ref currentQuestion.answers, ref currentQuestion.correctAnswerIndex);

		questionText.text = currentQuestion.questionText;
		for (int i = 0; i < answerButtons.Length; i++)
		{
			answerButtons[i].GetComponentInChildren<Text>().text = currentQuestion.answers[i];
		}

	}

	//Giving feedback for questioned answered
	//moving onto next question after a pause
	public void checkAnswer(int buttonNum)
	{
		Button selectedButton = answerButtons[buttonNum];

		if (allowSelection)
		{
			if (buttonNum == currentQuestion.correctAnswerIndex)
			{
				print("correct");
				numberOfCorrectAnswers++;
				feedbackText.GetComponent<Text>().text = "Correct";
				feedbackText.GetComponent<Text>().color = Color.green;
				correctSound.Play(); // Reproducir sonido correcto
									 // Iniciar la animación de vibración en el botón correcto
				HandleCorrectAnswer(answerButtons[buttonNum]);

				// Ocultar botones no seleccionados
				HideUnselectedButtons(selectedButton);
			}
			else
			{
				print("incorrect");
				feedbackText.GetComponent<Text>().text = "Incorrect";
				feedbackText.GetComponent<Text>().color = Color.red;
				incorrectSound.Play(); // Reproducir sonido incorrecto

				HandleIncorrectAnswer(buttonNum);

				// Ocultar botones no seleccionados
				HideUnselectedButtons(selectedButton);
			}
			StartCoroutine("continueAfterFeedback");
		}


	}

	//Choosing the question numbers for the trivia game
	void chooseQuestions()
	{
		for (int i = 0; i < questionNumbersChoosen.Length; i++)
		{
			int questionNum = Random.Range(0, questions.Length);
			if (numberNotContained(questionNumbersChoosen, questionNum))
			{
				questionNumbersChoosen[i] = questionNum;
			}
			else
			{
				i--;
			}

		}

		currentQuestionIndex = Random.Range(0, questions.Length);

	}

	// checking to see if the random number choosen has already been choosen
	bool numberNotContained(int[] numbers, int num)
	{

		for (int i = 0; i < numbers.Length; i++)
		{

			if (num == numbers[i])
			{
				return false;
			}
		}
		return true;
	}

	//assigns new question using the next question number
	public void moveToNextQuestion()
	{
		ShowAllButtons(); // Mostrar botones
		assignQuestion(questionNumbersChoosen[questionNumbersChoosen.Length - 1 - questionsFinished]);
	}

	// setting the results text to the appropiate text depending on how many questions were answered correctly
	void displayResults()
	{

		switch (numberOfCorrectAnswers)
		{
			case 5:
				resultsText.text = "5 out of 5 correct. You are all knowing!";
				break;
			case 4:
				resultsText.text = " 4 out of 5 correct. You are very smart!";
				break;
			case 3:
				resultsText.text = " 3 out of 5 correct. Well done.";
				break;
			case 2:
				resultsText.text = " 2 out of 5 correct. Better luck next time.";
				break;
			case 1:
				resultsText.text = " 1 out of 5 correct. You can do better than that!";
				break;
			case 0:
				resultsText.text = " 0 out of 5 correct. Are you even trying?";
				break;
			default:
				print("Not a correct number.");
				break;
		}
	}

	//restarts the level
	public void restartLevel()
	{
		//Application.LoadLevel(Application.loadedLevelName);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	//corutine that pauses and then moves onto next question.
	IEnumerator continueAfterFeedback()
	{
		allowSelection = false;
		feedbackText.SetActive(true);
		yield return new WaitForSeconds(1.0f);

		if (questionsFinished < questionNumbersChoosen.Length - 1)
		{
			moveToNextQuestion();
			questionsFinished++;
		}
		else
		{
			foreach (GameObject p in TriviaPanels)
			{
				p.SetActive(false);
			}
			finalResultsPanel.SetActive(true);
			displayResults();
		}
		allowSelection = true;
		feedbackText.SetActive(false);
	}

	void ShuffleAnswers(ref string[] answers, ref int correctAnswerIndex)
	{
		// Crear un array de índices
		int[] indices = new int[answers.Length];
		for (int i = 0; i < indices.Length; i++)
		{
			indices[i] = i;
		}

		// Mezclar los índices
		for (int i = 0; i < indices.Length; i++)
		{
			int randomIndex = Random.Range(0, indices.Length);
			int temp = indices[i];
			indices[i] = indices[randomIndex];
			indices[randomIndex] = temp;
		}

		// Crear un nuevo array para las respuestas mezcladas
		string[] shuffledAnswers = new string[answers.Length];
		int newCorrectAnswerIndex = 0;

		for (int i = 0; i < indices.Length; i++)
		{
			shuffledAnswers[i] = answers[indices[i]];
			if (indices[i] == correctAnswerIndex)
			{
				newCorrectAnswerIndex = i;
			}
		}

		// Asignar las respuestas mezcladas y el nuevo índice de respuesta correcta
		answers = shuffledAnswers;
		correctAnswerIndex = newCorrectAnswerIndex;
	}

	//checks for input to quit game
	void quitGame()
	{

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	public void HandleCorrectAnswer(Button button)
	{
		float shakeMagnitude = 10f; // Magnitud de la vibración
		float shakeDuration = 0.5f; // Duración de la vibración

		// Movimiento de vibración (hacia la izquierda y derecha)
		LeanTween.moveLocalX(button.gameObject, button.transform.localPosition.x + shakeMagnitude, shakeDuration / 10f)
			.setEaseShake()
			.setLoopPingPong(10); // 10 ciclos de ida y vuelta
	}

	public void HideUnselectedButtons(Button selectedButton)
	{
		foreach (Button btn in answerButtons)
		{
			if (btn != selectedButton)
			{
				LeanTween.scale(btn.gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInBack);
			}
		}
	}

	public void ShowAllButtons()
	{
		foreach (Button btn in answerButtons)
		{
			LeanTween.scale(btn.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);
		}
	}

	public void HandleIncorrectAnswer(int buttonIndex)
	{
		// Escalar el botón a un tamaño más pequeño y luego restaurarlo
		LeanTween.scale(answerButtons[buttonIndex].gameObject, new Vector3(0.8f, 0.8f, 0.8f), 0.1f)
			.setEase(LeanTweenType.easeOutBounce) // Efecto de rebote
			.setLoopPingPong(1); // Hace que la animación vuelva al tamaño original
	}
}
