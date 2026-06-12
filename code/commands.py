import os
import datetime
import wikipedia
import pywhatkit
from utils import speak

def open_chrome():
    speak("Opening Chrome")
    os.system("start chrome")

def open_notepad():
    speak("Opening Notepad")
    os.system("notepad")

def tell_time():
    time = datetime.datetime.now().strftime("%H:%M")
    speak(f"The time is {time}")

import os
import random
from utils import speak

def play_music(command=""):
    music_dir = "music"
    songs = os.listdir(music_dir)

    if not songs:
        speak("No music found")
        return

    # 🎯 Play specific song by name
    if command:
        for song in songs:
            if command.lower() in song.lower():
                os.startfile(os.path.join(music_dir, song))
                speak(f"Playing {song}")
                return

    # 🔀 Otherwise play random song
    song = random.choice(songs)
    os.startfile(os.path.join(music_dir, song))
    speak(f"Playing {song}")

def search_google(query):
    speak("Searching Google")
    pywhatkit.search(query)

def search_wikipedia(query):
    speak("Searching Wikipedia")
    result = wikipedia.summary(query, sentences=2)
    speak(result)

def open_custom_project():
    speak("Opening your project")
    os.startfile("D:\\your_project_folder")  # CHANGE THIS