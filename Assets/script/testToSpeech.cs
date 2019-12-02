using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Text;
using System.IO;

public class testToSpeech : MonoBehaviour
{
    private AudioSource soundPlayer;

    public string final_word;
    public string korean_translation;

    private word[] word_sequence;
    private word[] objects;

    void Start()
    {
        soundPlayer = GetComponent<AudioSource>();
        word_sequence = new word[30];

        final_word = "";
        korean_translation = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            WordCombination();

            if (!final_word.Equals(""))
            {                
                WordTranslation();
                TextToSpeechRequest();
                KoreanTextToSpeechRequest();
                SoundPlay();
            }
        }
    }

    void WordCombination()
    {
        //objects = GameObject.FindObjectsOfType<GameObject>();

        objects = FindObjectsOfType(typeof(word)) as word[];

        int index = 0;
        word temp;
        int i, j;

        foreach (var one in objects)
        {
            if (one.enabled)
            {
                Debug.Log("word : "+one.myWord+", position : "+one.transform.position);
                word_sequence[index] = one;

                index++;
                //30자 이상은 돌아가
                if (index >= 30)
                {
                    index = 0;
                    return;
                }
            }
        }
        
        //위치비교 후 정렬
        for(i = 0; i < index-1; i++){
            for(j = i+1; j < index; j++){
                if (word_sequence[i].transform.position.x > word_sequence[j].transform.position.x){

                    temp = word_sequence[i];
                    word_sequence[i] = word_sequence[j];
                    word_sequence[j] = temp;
                }
            }
            Debug.Log(word_sequence[i].myWord);
        }
        

        //단어 조합
        final_word = "";
        for (i = 0; i < index; i++){
            final_word += word_sequence[i].myWord;
        }

        //Debug.Log("final_word : " + final_word);
    }

    //번역 en->kr
    void WordTranslation()
    {
        string clientId = "";
        string clientSecretId = "";

        string text = final_word;
        string result;

        string url = "https://openapi.naver.com/v1/papago/n2mt";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Headers.Add("X-Naver-Client-Id", clientId);
        request.Headers.Add("X-Naver-Client-Secret", clientSecretId);
        request.Method = "POST";

        byte[] byteDataParams = Encoding.UTF8.GetBytes("source=en&target=ko&text=" + text);
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = byteDataParams.Length;
        Stream st = request.GetRequestStream();
        st.Write(byteDataParams, 0, byteDataParams.Length);
        st.Close();
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream stream = response.GetResponseStream();
        StreamReader reader = new StreamReader(stream, Encoding.UTF8);

        result = reader.ReadToEnd();

        stream.Close();
        response.Close();
        reader.Close();

        int idx = result.IndexOf("translatedText");

        result = result.Substring(idx + 17);
        result = result.Substring(0, result.LastIndexOf('"'));

        korean_translation = result;
        Debug.Log("Translation : " + korean_translation);

    }

    //Naver TTS(en mp3)
    void TextToSpeechRequest()
    {
        string clientId = "";
        string clientSecretId = "";

        string speaker = "clara";
        string speed = "0";

        string text = final_word; // 음성합성할 문자값

        string url = "https://naveropenapi.apigw.ntruss.com/voice/v1/tts";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        request.Headers.Add("X-NCP-APIGW-API-KEY-ID", clientId);
        request.Headers.Add("X-NCP-APIGW-API-KEY", clientSecretId);
        request.Method = "POST";

        byte[] byteDataParams = Encoding.UTF8.GetBytes("speaker="+speaker+"&speed="+speed+"&text=" + text);

        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = byteDataParams.Length;

        Stream st = request.GetRequestStream();
        st.Write(byteDataParams, 0, byteDataParams.Length);
        st.Close();

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        string status = response.StatusCode.ToString();

        //있으면 제거!
        if (File.Exists(Application.dataPath + "/tts.mp3"))
            File.Delete(Application.dataPath + "/tts.mp3");

        using (Stream output = File.OpenWrite(Application.dataPath+ "/tts.mp3"))
        using (Stream input = response.GetResponseStream()){
            input.CopyTo(output);
        }
        Debug.Log("tts.mp3 was created");
    }

    //Kakao TTS(kr mp3)
    void KoreanTextToSpeechRequest()
    {
        string clientId = "";

        string text = korean_translation;

        string url = "https://kakaoi-newtone-openapi.kakao.com/v1/synthesize";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        
        request.Headers.Add("Authorization", clientId);
        request.Method = "POST";

        byte[] byteDataParams = Encoding.UTF8.GetBytes("<speak><voice>" + text + "</voice></speak>");
        
        request.ContentType = "application/xml";
        //request.ContentLength = byteDataParams.Length;

        Stream st = request.GetRequestStream();
        st.Write(byteDataParams, 0, byteDataParams.Length);
        st.Close();

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        string status = response.StatusCode.ToString();
        Debug.Log("status : "+status);

        //있으면 제거!
        if (File.Exists(Application.dataPath + "/tts_korean.mp3"))
            File.Delete(Application.dataPath + "/tts_korean.mp3");

        using (Stream output = File.OpenWrite(Application.dataPath + "/tts_korean.mp3"))
        using (Stream input = response.GetResponseStream()){
            input.CopyTo(output);
        }
        Debug.Log("tts_korean.mp3 was created");

        
         

    }
    void SoundPlay()
    {
       
        bool en_is_exists = File.Exists(Application.dataPath + "/tts.mp3");
        bool kr_is_exists = File.Exists(Application.dataPath + "/tts_korean.mp3");
        Debug.Log("en : "+en_is_exists+", kr : "+kr_is_exists);


        if (en_is_exists && kr_is_exists)
        {
            StartCoroutine(LoadSongCoroutine_English());

            StartCoroutine(LoadSongCoroutine_Korean());
        }

        //if (){
            //readfile = new WWW("file:/"+Application.dataPath+"/tts_korean.mp3");
            //sound = readfile.GetAudioClip();

            //sound = (AudioClip)Resources.Load("tts_korean");
            //sound = soundPlayer.clip.GetData()
            //soundPlayer.clip = sound;
            //soundPlayer.PlayOneShot(sound);

            //if (!soundPlayer.isPlaying){
            //sound_korean = (AudioClip)Resources.Load("tts_korean");
            //soundPlayer.PlayOneShot(sound_korean);
            //}
            //File.Delete(Application.dataPath + "/Resources/tts_korean.mp3");
        //}
        

    }

    IEnumerator LoadSongCoroutine_Korean()
    {
        string path = "file://" + Application.dataPath + "/tts_korean.mp3";

        WWW www = new WWW(path);
        yield return www;

        AudioClip korean_sound = www.GetAudioClip(false, false, AudioType.MPEG);
        soundPlayer.clip = korean_sound;
        soundPlayer.PlayDelayed(0.7f);

    }

    IEnumerator LoadSongCoroutine_English()
    {
        string path = "file://" + Application.dataPath + "/tts.mp3";

        WWW www = new WWW(path);
        yield return www;

        AudioClip english_sound = www.GetAudioClip(false, false, AudioType.MPEG);
        soundPlayer.PlayOneShot(english_sound);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 200, 50), "English : " + final_word);
        GUI.Label(new Rect(20, 40, 200, 50), "Korean : "+ korean_translation);
    }

}
