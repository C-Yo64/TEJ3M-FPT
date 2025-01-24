/* 
Bluetooth motion controller
By: Carter
Date: Jan. 22, 2025
This is a bluetooth controller for a game that utilizes motion controls using a gyro
*/

#include <Adafruit_MPU6050.h>
#include <Adafruit_Sensor.h>
#include <Wire.h>
#include <BluetoothSerial.h>

Adafruit_MPU6050 mpu;
BluetoothSerial BT;

// int led_pin = 13;
// The pin for the blue built-in LED
int LED_BUILTIN = 2;

// The angle along the X axis read by the gyro
float angleX = 0.0;
// The angle along the Y axis read by the gyro
float angleY = 0.0;
// The angle along the Z axis read by the gyro
float angleZ = 0.0;

// The amount of time the previous loop took to run
unsigned long lastTime = 0;

// The pin of the LED representing "up"
int LED1 = 18;
// The pin of the LED representing "down"
int LED2 = 19;
// The pin of the LED representing "left"
int LED3 = 23;
// The pin of the LED representing "right"
int LED4 = 4;

// The pin connected directly to the capacitor to read the charge
int capPin = 32;
// The pin used to read the button
int buttonPin = 35;
// The pin connected to the capacitor through a resistor used to charge/discharge it
int capChargePin = 33;

// The pin used to read the gyro recenter button
int resetButtonPin = 34;

// The amount of time it takes for the code to run
float deltaTime;

void setup() {
  // put your setup code here, to run once:

  // Start serial communication on the baud rate 9600
  Serial.begin(9600);
  // Block the thread until the serial has started
  while (!Serial) {
    delay(10);
  }
  // pinMode(led_pin, OUTPUT);
  // Configure the built-in LED's pin for output
  pinMode(LED_BUILTIN, OUTPUT);

  // Configure the GPIO pins for the button, capacitor, and the resistors connected to the capactior to charge it
  pinMode(buttonPin, INPUT);
  pinMode(capChargePin, OUTPUT);
  pinMode(capPin, INPUT);

  // Configure the pin for the gyro recenter button
  pinMode(resetButtonPin, INPUT);

  // Make sure the built-in LED is off (it will act as an indicator for if the gyro and bluetooth are activated)
  digitalWrite(LED_BUILTIN, LOW);

  // Try to start the gyro and if it fails, print that to the serial monitor and stop the thread from running
  if (!mpu.begin()) {
    Serial.println("Failed to start MPU6050");
    while (true) {
      delay(10);
    }
  }

  // Try to start bluetooth and if it fails, print that to the serial monitor and stop the thread from running
  if (!BT.begin("ESP32")) {
    Serial.println("Failed to start Bluetooth");
    while (true) {
      delay(10);
    }
  }

  // Once everything has been started properly, light the built-in LED to show that
  digitalWrite(LED_BUILTIN, HIGH);
}

void loop() {
  // put your main code here, to run repeatedly:

  // Calculate deltaTime (the time it takes for the code to run)
  // This is used to make sure the gyro readings are accurate and not affected by lag
  unsigned long currentTime = millis();
  deltaTime = (currentTime - lastTime) / 1000.0;  // Convert to seconds
  lastTime = currentTime;

  // Get the values from the gyro and light the LEDs corresponding to the direction
  readGyro();
  lightLEDs();

  // Charge/discharge the capacitor based on the button pressed, and read its charge
  chargeCapacitor();

  // Add a buffer to make the bluetooth serial printing more reliable
  // Without this, messages are sometimes dropped
  delay(50);
}

// Reads the gyro values and updates the angle
void readGyro() {
  // Get the current readings from the gyro
  sensors_event_t a, g, temp;
  mpu.getEvent(&a, &g, &temp);

  // Add a deadzone to account for the gyro drifting so the angle doesn't constantly build up
  // Once past the deadzone, add the current reading the stored angle to keep track of how it's rotated (using deltaTime to make sure it's  accurate/not affected by lag)
  // X and Y were swapped later to account for the angle of the gyro when it was in the case (swapping it here was the quickest and easiest way)
  if (g.gyro.x < -0.05 || g.gyro.x > 0.05) {
    angleY += g.gyro.x * deltaTime;
  }
  if (g.gyro.y < -0.02 || g.gyro.y > 0.02) {
    angleX += g.gyro.y * deltaTime;
  }
  if (g.gyro.x < -0.05 || g.gyro.z > 0.05) {
    angleZ += g.gyro.z * deltaTime;
  }

  // If the recenter button is pressed or if the serial/bluetooth communication reads '1' (This is sent if the mouse is clicked in the game)
  // Reset all of the angles
  if (Serial.read() == '1' || BT.read() == '1' || digitalRead(resetButtonPin) == HIGH) {
    angleX = 0.0;
    angleY = 0.0;
    angleZ = 0.0;
  }

  // Combine all the gyro readings into one string, and send it to the serial/bluetooth
  // Everything has to be on one line to account for the bandwidth limitations of bluetooth
  String message = "X: " + String(angleX) + ", Y: " + String(angleY) + ", Z: " + String(angleZ);
  Serial.println(message + ", ");
  BT.print(message + ", ");
}

// Light the direction LEDs based on the gyro angle
void lightLEDs() {
  // Add a deadzone in the middle where no LEDs will light
  // Once it passes the initial deadzone, the LED will light at half brightness using PWM to control it, after the angle in that direction increases even more, light it at full brightness
  // Up LED
  if (angleY >= 0.05 && angleY <= 0.74) {
    analogWrite(LED1, 64);
  } else if (angleY >= 0.75) {
    analogWrite(LED1, 255);
  } else {
    analogWrite(LED1, 0);
  }

  // Down LED
  if (angleY <= -0.05 && angleY >= -0.74) {
    analogWrite(LED2, 64);
  } else if (angleY <= -0.75) {
    analogWrite(LED2, 255);
  } else {
    analogWrite(LED2, 0);
  }

  // Left LED
  if (angleX <= -0.05 && angleX >= -0.74) {
    analogWrite(LED3, 64);
  } else if (angleX <= -0.75) {
    analogWrite(LED3, 255);
  } else {
    analogWrite(LED3, 0);
  }

  // Right LED
  if (angleX >= 0.05 && angleX <= 0.74) {
    analogWrite(LED4, 64);
  } else if (angleX >= 0.75) {
    analogWrite(LED4, 255);
  } else {
    analogWrite(LED4, 0);
  }
}

// Uses the button state to either charge or discharge the capacitor, and reads its value
void chargeCapacitor() {
  // Read and print the capacitor charge, and complete the line started by the gyro function
  Serial.println("Charge: " + String(analogRead(capPin)));
  BT.println("Charge: " + String(analogRead(capPin)));

  // If the button is pressed, start sending power to the capacitor to charge it
  if (digitalRead(buttonPin) == HIGH) {
    digitalWrite(capChargePin, HIGH);
    Serial.println("charging");
  }
  // If the button isn't pressed, the pin will output ground and start discharging the capacitor
  if (digitalRead(buttonPin) == LOW) {
    digitalWrite(capChargePin, LOW);
    Serial.println("discharging");
  }
}
