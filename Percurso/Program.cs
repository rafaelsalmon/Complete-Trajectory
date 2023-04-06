using System;
using System.IO.Ports;
using Camera;
using Arquivos;

//This app/program performs Trajectory Control and orchestrates two other projects as libraries: Files ("Arquivos") and Camera ("Camera")

// This is responsible for moving a camera from its initial position, stopping in front of each gondola's basket and its label, until it has finished the whole route/trajectory.
// In this current version, it has been projected for a single gondola module (width = 1,1 meter and three levels of baskets) so it can be easily demonstrated.
// Every level of baskets has two divisions (forming three baskets per level).

SerialPort _serialPort;

Console.WriteLine("Digite a porta:");
//Listen to port//

//Collect, from the user, the port being used to communicate from the Raspberry Pi with the proprietary board (there is a Utility Console App to list the ports in the system);
string port = "/dev/tty" + Console.ReadLine(); //Currently in Debian Linux format. //ACM0 is the format and expected port if testing on Windows; 
const string baudrate = "115200"; //Default and constant
_serialPort = new SerialPort(port, int.Parse(baudrate));

// Set the read/write timeouts
_serialPort.ReadTimeout = 150000;
_serialPort.WriteTimeout = 1500000;

// Set path parameters
string esquerda = "L"; //Esquerda //Left
string direita = "R"; //Direita //Right
string cima = "U"; //Up
string baixo = "D"; //Down
string fast = "001"; //001 represents a fast movement
string slow = "040"; //040 represents a slow movement
double distancia_entre_etiquetas = 33 + 3;//cm (22 pra testes menores) //Distance between labels
double distancia_etiqueta_lateral_cesto = 16.5;//centimeters //Dustance between a label and the edge of a basket (each level of the gondola has three baskets of products)
double altura_entre_andares = 39.5;// centimeters // height between levels ("floors") of the gondola
double passos_por_cm_horizontal = 47.62; // how many steps of a motor are needed to move a camera one centimeter

_serialPort.Open();

Percurso(_serialPort);

_serialPort.Close();


AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

//Controls all the trajectory
void Percurso(SerialPort _serialPort)
{
        UmaLinha(_serialPort, direita); //Moves the camera one line (the lengh of a level/floor) to the right
        MoveColumn(_serialPort, baixo, altura_entre_andares); //Desce um andar //Moves the camera one level below, maintaining it in the same column
        UmaLinha(_serialPort, esquerda); //Moves the camera one line to the left
        MoveColumn(_serialPort, cima, altura_entre_andares); //Moves the camera one level above along a column
        Arquivos.Arquivos.Send(); //Send files to the cloud via api
}

//Moves the camera horizontally and photographs labels
void UmaLinha(SerialPort _serialPort, string sentido)
{

    Camera.Camera cam = new Camera.Camera();
    Thread.Sleep(5000);
    string retorno = cam.Fotografa(); //Photographs label
    Thread.Sleep(5000);

    //Da primeira para a segunda etiqueta //From the first to the second label
    MoveLine(_serialPort, sentido, distancia_entre_etiquetas); 
    Thread.Sleep(5000);
    retorno = cam.Fotografa();
    Thread.Sleep(5000);
    
    MoveLine(_serialPort, sentido, distancia_entre_etiquetas); //Da segunda para a terceira etiqueta
    
    Thread.Sleep(5000);
    retorno = cam.Fotografa(); //Photographs label
    Thread.Sleep(5000); 
}

//Moves the camera vertically
void UmAndar(SerialPort _serialPort, string sentido)
{
    MoveLine(_serialPort, sentido, distancia_etiqueta_lateral_cesto); //Da lateral esquerda/direita até o meio da etiqueta
}

//Moves the camera up or down a certain distance
void MoveColumn(SerialPort _serialPort, string sentido, double distancia)
{
    string speed = "";
    
    if(sentido == cima) {
        speed = slow; //Moves slowly up for mechanical reasons
    }
    else if (sentido == baixo){
        speed = fast; //Moves fastly down
    }

    Move(_serialPort, sentido, distancia, speed); //Moves in a certain direction, distance and speed.
}

//Moves a certain distance to the left or to the right

void MoveLine(SerialPort _serialPort, string sentido, double distancia)
{

    string speed = fast;

    Move(_serialPort, sentido, distancia, speed);
}

//Moves a certain distance in any given direction with a certain speed
void Move(SerialPort _serialPort, string sentido, double distancia, string velocidade)
{
    string message = "";
    string steps = CalculaPassosHorizontais(distancia); //Calculate number of horizontal steps for a certain distance
    string separator = ","; //Part of the syntax expected by the firmware of the custom board
    string speed = velocidade;
    string terminator = "\r\n"; //Part of the syntax expected by the firmware of the custom board
    string starter = "\r\n"; //Part of the syntax expected by the firmware of the custom board

    //TODO melhorar:
    if ((sentido == "baixo") || (sentido == "cima")){
        int st = int.Parse(steps);
        st -= 150; //ajuste
        st -= 250; //ajuste
        steps = st.ToString();
    }
    message = starter + sentido + steps + separator + speed + terminator; //Mounts command to the firmware of the custom board
    Console.WriteLine(message); //Logging purposes
    Caminha(_serialPort, message); //Sends message / performs movement
}

//Calculates horizontal steps of the motor per distance
string CalculaPassosHorizontais(double distanciaHorizontal) {
    double passosHorizontais = distanciaHorizontal * passos_por_cm_horizontal;
    int passos = (int) passosHorizontais;
    string passosFormatados = passos.ToString("D5");
    return passosFormatados;
}

//Performs the movement by sending the command/message through the serial port
static void Caminha(SerialPort _serialPort, string message)
{
    try
    {
        _serialPort.WriteLine(message);
    }
    catch (Exception e)
    {
        int i = 1; 
        _serialPort.Close();
        Console.WriteLine("Gerou exception:" + e.Message + "   + INTERNA: " + e.InnerException);
    }
}

void CurrentDomain_ProcessExit(object sender, EventArgs e)
{
    _serialPort.Close();
    Console.WriteLine("Saiu com Process Exit"); //Message reads "Exited with Process Exit"

}