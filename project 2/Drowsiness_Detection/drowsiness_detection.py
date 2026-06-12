import cv2
import numpy as np
from scipy.spatial import distance
from playsound import playsound
from mediapipe.tasks.python import BaseOptions
from mediapipe.tasks.python.vision import FaceLandmarker, FaceLandmarkerOptions, RunningMode
from mediapipe import Image, ImageFormat  # Correct import for Image and ImageFormat

# ---------------- PARAMETERS ----------------
EYE_AR_THRESH = 0.25
EYE_AR_CONSEC_FRAMES = 20
COUNTER = 0
ALARM_FILE = "alarm.wav"  # Path to your alarm sound

# ---------------- SETUP MEDIAPIPE ----------------
base_options = BaseOptions(model_asset_path="face_landmarker.task")  # Download this .task file from MediaPipe
options = FaceLandmarkerOptions(
    base_options=base_options,
    running_mode=RunningMode.VIDEO,
    num_faces=1
)
landmarker = FaceLandmarker.create_from_options(options)

# Eye landmark indices
LEFT_EYE = [33, 160, 158, 133, 153, 144]
RIGHT_EYE = [362, 385, 387, 263, 373, 380]

def eye_aspect_ratio(eye):
    A = distance.euclidean(eye[1], eye[5])
    B = distance.euclidean(eye[2], eye[4])
    C = distance.euclidean(eye[0], eye[3])
    return (A + B) / (2.0 * C)

cap = cv2.VideoCapture(0)
frame_count = 0

while True:
    ret, frame = cap.read()
    if not ret:
        break

    rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)

    # Correct way to create MediaPipe Image object
    mp_image = Image(image_format=ImageFormat.SRGB, data=rgb_frame)

    results = landmarker.detect_for_video(mp_image, timestamp_ms=frame_count)
    frame_count += 33  # Approx. 30 FPS

    if results.face_landmarks:
        for face in results.face_landmarks:
            h, w, _ = frame.shape
            leftEye = []
            rightEye = []

            for idx in LEFT_EYE:
                x = int(face[idx].x * w)
                y = int(face[idx].y * h)
                leftEye.append((x, y))
                cv2.circle(frame, (x, y), 2, (0, 255, 0), -1)

            for idx in RIGHT_EYE:
                x = int(face[idx].x * w)
                y = int(face[idx].y * h)
                rightEye.append((x, y))
                cv2.circle(frame, (x, y), 2, (0, 255, 0), -1)

            ear = (eye_aspect_ratio(leftEye) + eye_aspect_ratio(rightEye)) / 2.0

            if ear < EYE_AR_THRESH:
                COUNTER += 1
                if COUNTER >= EYE_AR_CONSEC_FRAMES:
                    cv2.putText(frame, "DROWSINESS ALERT!", (10, 30),
                                cv2.FONT_HERSHEY_SIMPLEX, 0.7, (0, 0, 255), 2)
                    playsound(ALARM_FILE)
            else:
                COUNTER = 0

            cv2.putText(frame, f"EAR: {ear:.2f}", (300, 30),
                        cv2.FONT_HERSHEY_SIMPLEX, 0.7, (255, 255, 255), 2)

    cv2.imshow("Drowsiness Detection", frame)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()