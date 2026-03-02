using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneratorPodloza : MonoBehaviour
{
    public Camera kameraGlowne;
    public Transform punktStartowy;                 // Punkt, od ktorego rozpocznie sie generowanie naszego podloza
    public KafelekPlatformy prefabKafelka;
    public float predkoscRuchu = 12;
    public int liczbaWstepnychKafelkow = 15;       // Ile kafelkow ma byc wygenerowanych na starcie naszej gry
    public int kafelkiBezPrzeszkod = 3;            // Ile pierwszych kafelkow ma byc bez przeszkod

    List<KafelekPlatformy> wygenerowaneKafelki = new List<KafelekPlatformy>();
    int indeksKafelkaDoAktywacji = -1;

    [HideInInspector]                             //Zmienna musi byc publiczna, ale nie chcemy zeby podczas projektowania gry przypadkiem zmienic ja w Inspectorze UNITY
    public bool graSkonczona = false;
    static bool graRozpoczeta = false;
    float wynik = 0;
    public static GeneratorPodloza instancja;

    [Header("Automatyczna zmiana")]
    public int progWynikuDoZmianySceny = 1000;          
    public string nazwaKolejnejSceny;                   
    bool scenaJuzZmieniona = false;                     

    void Start()
    {
        instancja = this;

        Vector3 pozycjaGenerowania = punktStartowy.position;
        int tymczasoweKafelkiBezPrzeszkod = kafelkiBezPrzeszkod;

        for (int i = 0; i < liczbaWstepnychKafelkow; i++)
        {
            pozycjaGenerowania -= prefabKafelka.punktStartowy.localPosition;
            KafelekPlatformy kafelek = Instantiate(prefabKafelka, pozycjaGenerowania, Quaternion.identity);

            if (tymczasoweKafelkiBezPrzeszkod > 0)
            {
                kafelek.DezaktywujWszystkiePrzeszkody();
                tymczasoweKafelkiBezPrzeszkod--;
            }
            else
            {
                kafelek.AktywujLosowaPrzeszkode();
            }

            pozycjaGenerowania = kafelek.punktKoncowy.position;
            kafelek.transform.SetParent(transform);
            wygenerowaneKafelki.Add(kafelek);
        }
    }

    void Update()
    {
        if (!graSkonczona && graRozpoczeta)
        {
            transform.Translate(
                -wygenerowaneKafelki[0].transform.forward *                //wektor kierunku pierwszego kafelka
                Time.deltaTime * (predkoscRuchu + (wynik / 500)),        //czas od ostatniej klatki (umozliwia plynny ruch niezaleznie od FPS)
                Space.World                                                 //predkosc, ktora rosnie wraz z wynikiem = wiekszy wynik, tym szybciej obiekt sie porusza
            );

            wynik += Time.deltaTime * predkoscRuchu;

            if (!scenaJuzZmieniona && progWynikuDoZmianySceny > 0 && wynik >= progWynikuDoZmianySceny)
            {
                scenaJuzZmieniona = true;          // zabezpieczenie przed wywolywaniem sceny w kazdej klatce
                if (!string.IsNullOrEmpty(nazwaKolejnejSceny))
                {
                    SceneManager.LoadScene(nazwaKolejnejSceny);
                }
            }


        }

        if (kameraGlowne.WorldToViewportPoint(wygenerowaneKafelki[0].punktKoncowy.position).z < 0)
        {
            KafelekPlatformy kafelek = wygenerowaneKafelki[0];
            wygenerowaneKafelki.RemoveAt(0);

            kafelek.transform.position =
                wygenerowaneKafelki[wygenerowaneKafelki.Count - 1].punktKoncowy.position
                - kafelek.punktStartowy.localPosition;

            kafelek.AktywujLosowaPrzeszkode();
            wygenerowaneKafelki.Add(kafelek);
        }

        if (graSkonczona || !graRozpoczeta)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (graSkonczona)
                {
                    Scene scena = SceneManager.GetActiveScene();
                    //SceneManager.LoadScene(scena.name);
                    SceneManager.LoadScene("TheEnd");
                    PlayerPrefs.SetInt("Wynik gry", (int)wynik);       //zapis wyniku do schowka, dla klucza "Wynik gry"
                    PlayerPrefs.Save();                                //wymuszony zapis na dysku, przerzut miedzy sesjami w Unity

                }
                else
                {
                    graRozpoczeta = true;
                }
            }
        }
    }

    void OnGUI()
    {

        GUI.skin.label.fontSize = 24;                   // 2 razy wieksza czcionka, default 12pxs

        if (graSkonczona)
    {
        GUI.color = Color.red;
        GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 300, 200),
            "Koniec gry\nTwoj wynik: " + ((int)wynik) + "\nNacisnij SPACJE aby zrestartować");
    }
    else
    {
        if (!graRozpoczeta)
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 300, 400),
                "SPACJA - START");
        }
    }

    GUI.color = Color.green;
    GUI.Label(new Rect(5, 5, 300, 150), "Wynik: " + ((int)wynik));
    }
}
