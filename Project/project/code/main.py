from utils import speak, listen
import commands

def run_alexa():
    speak("Hello, I am Alexa. How can I help you?")

    while True:
        command = listen()

        if command == "":
            continue

        # 🔓 OPEN APPLICATIONS
        elif any(word in command for word in ["open chrome", "start chrome", "launch chrome"]):
            commands.open_chrome()

        elif any(word in command for word in ["open notepad", "start notepad"]):
            commands.open_notepad()

        elif any(word in command for word in ["open news", "start news"]):
            commands.open_news()

        elif "open my project" in command:
            commands.open_custom_project()

        # ⏰ TIME & DATE
        elif "time" in command:
            commands.tell_time()

        elif "date" in command or "today" in command:
            commands.tell_date()

        # 🎵 MUSIC
        elif any(word in command for word in ["play music", "play song", "start music"]):
            commands.play_music()

        # 🌐 GOOGLE SEARCH
        elif "search google" in command or "google" in command:
            speak("What should I search?")
            query = listen()
            commands.search_google(query)

        # 📚 WIKIPEDIA
        elif "wikipedia" in command or "who is" in command or "what is" in command:
            speak("What should I search on Wikipedia?")
            query = listen()
            commands.search_wikipedia(query)

        # 🎬 YOUTUBE
        elif "youtube" in command:
            speak("What should I play on YouTube?")
            query = listen()
            commands.play_youtube(query)

        # 📂 OPEN FILE/FOLDER
        elif "open folder" in command:
            speak("Tell me folder name")
            folder = listen()
            commands.open_folder(folder)

        # 💬 SMALL TALK
        elif "how are you" in command:
            speak("I am fine, thank you!")

        elif "your name" in command:
            speak("I am Alexa, your assistant")

        elif "hello" in command or "hi" in command:
            speak("Hello! How can I help you?")

        # 🔊 VOLUME CONTROL
        elif "volume up" in command:
            commands.volume_up()

        elif "volume down" in command:
            commands.volume_down()

        # 🔐 EXIT
        elif any(word in command for word in ["exit", "stop", "quit", "bye"]):
            speak("Goodbye!")
            break

        # ❌ UNKNOWN COMMAND
        else:
            speak("Sorry, I didn't understand that. Please try again.")

if __name__ == "__main__":
    run_alexa()