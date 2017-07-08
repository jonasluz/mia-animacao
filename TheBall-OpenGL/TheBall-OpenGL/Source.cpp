/*
Source for OpenGL code: http://www.inf.pucrs.br/~manssour/OpenGL/Tutorial.html
*/

#include <windows.h>
#include <iostream>
#include <stdlib.h>
#include <math.h>
#include <gl/freeglut.h>
#include "imageloader.h"

using namespace std;

#define PI 3.14159265

GLfloat angle, fAspect;
GLdouble obsX = 0, obsY = 0, obsZ = 400;

bool flag = 0;
float ball_x = -200.0, ball_y = 0.0, ball_z = 0.0;
float valueAngle = 0, maxSin = 360;

bool anima = false, reset = false;

GLuint _textureId; //The id of the texture
GLUquadric *quad;
float rotate;


//Makes the image into a texture, and returns the id of the texture
GLuint loadTexture(Image* image) {
	GLuint textureId;
	glGenTextures(1, &textureId); //Make room for our texture
	glBindTexture(GL_TEXTURE_2D, textureId); //Tell OpenGL which texture to edit
											 //Map the image to the texture
	glTexImage2D(GL_TEXTURE_2D,                //Always GL_TEXTURE_2D
		0,                            //0 for now
		GL_RGB,                       //Format OpenGL uses for image
		image->width, image->height,  //Width and height
		0,                            //The border of the image
		GL_RGB, //GL_RGB, because pixels are stored in RGB format
		GL_UNSIGNED_BYTE, //GL_UNSIGNED_BYTE, because pixels are stored
						  //as unsigned numbers
		image->pixels);               //The actual pixel data
	return textureId; //Returns the id of the texture
}

// Função callback chamada para fazer o desenho
void Desenha(void)
{
	// Limpa a janela e o depth buffer
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

	glEnable(GL_TEXTURE_2D);
	glBindTexture(GL_TEXTURE_2D, _textureId);

	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);

	//glColor3f(0.0f, 0.0f, 1.0f);

	glPushMatrix();
	glTranslatef(ball_x, ball_y, ball_z);
	glRotatef(90, 1.0f, 0.0f, 0.0f);
	gluQuadricTexture(quad, 1);
	gluSphere(quad, 30, 25, 25);
	//	glutSolidSphere(10, 25, 25);
	glPopMatrix();

	glutSwapBuffers();
}

// Função callback chamada pela GLUT a cada intervalo de tempo
void Timer(int value)
{
	if (reset) {
		ball_x = -200.0;
		ball_y = 0.0;

		anima = false;
		reset = false;

		obsX = 0, obsY = 0, obsZ = 400;
		flag = false;
		maxSin = 360;
		valueAngle = 0;

		glLoadIdentity();
		gluLookAt(obsX, obsY, obsZ, 0, 0, 0, 0, 1, 0);

	}
	else if (anima) {

		if (!flag) {

			ball_y = fabs(sin(valueAngle*PI / 180)) * 40;

			valueAngle += 20;

			if (valueAngle == maxSin) {
				maxSin -= 60;

				flag = true;
			}

		}
		else {

			ball_y = fabs(sin(valueAngle*PI / 180)) * 40;

			valueAngle -= 20;

			if (valueAngle <= 0) {
				flag = false;
			}

			if (maxSin == 0 && valueAngle == 0) {
				anima = false;
			}
		}

		ball_x += 5;

		obsX += 5;

		glLoadIdentity();
		gluLookAt(obsX, obsY, obsZ, 0, 0, 0, 0, 1, 0);

	}

	glutPostRedisplay();
	glutTimerFunc(33, Timer, 1);
}

// Inicializa parâmetros de rendering
void Inicializa(void)
{
	GLfloat luzAmbiente[4] = { 0.2,0.2,0.2,1.0 };
	GLfloat luzDifusa[4] = { 0.7,0.7,0.7,1.0 };	   // "cor" 
	GLfloat luzEspecular[4] = { 1.0, 1.0, 1.0, 1.0 };// "brilho" 
	GLfloat posicaoLuz[4] = { 0.0, 50.0, 50.0, 1.0 };

	// Capacidade de brilho do material
	GLfloat especularidade[4] = { 1.0,1.0,1.0,1.0 };
	GLint especMaterial = 60;

	// Especifica que a cor de fundo da janela será preta
	glClearColor(0.0f, 0.0f, 0.0f, 1.0f);

	// Habilita o modelo de colorização de Gouraud
	glShadeModel(GL_SMOOTH);

	// Define a refletância do material 
	glMaterialfv(GL_FRONT, GL_SPECULAR, especularidade);
	// Define a concentração do brilho
	glMateriali(GL_FRONT, GL_SHININESS, especMaterial);

	// Ativa o uso da luz ambiente 
	glLightModelfv(GL_LIGHT_MODEL_AMBIENT, luzAmbiente);

	// Define os parâmetros da luz de número 0
	glLightfv(GL_LIGHT0, GL_AMBIENT, luzAmbiente);
	glLightfv(GL_LIGHT0, GL_DIFFUSE, luzDifusa);
	glLightfv(GL_LIGHT0, GL_SPECULAR, luzEspecular);
	glLightfv(GL_LIGHT0, GL_POSITION, posicaoLuz);

	// Habilita a definição da cor do material a partir da cor corrente
	glEnable(GL_COLOR_MATERIAL);
	//Habilita o uso de iluminação
	glEnable(GL_LIGHTING);
	// Habilita a luz de número 0
	glEnable(GL_LIGHT0);
	// Habilita o depth-buffering
	glEnable(GL_DEPTH_TEST);

	glEnable(GL_NORMALIZE);

	quad = gluNewQuadric();

	angle = 45;

	Image* image = loadBMP("earth.bmp");
	_textureId = loadTexture(image);
	delete image;
}

// Função usada para especificar o volume de visualização
void EspecificaParametrosVisualizacao(void)
{
	// Especifica sistema de coordenadas de projeção
	glMatrixMode(GL_PROJECTION);
	// Inicializa sistema de coordenadas de projeção
	glLoadIdentity();

	// Especifica a projeção perspectiva
	gluPerspective(angle, fAspect, 0.4, 1000);

	// Especifica sistema de coordenadas do modelo
	glMatrixMode(GL_MODELVIEW);
	// Inicializa sistema de coordenadas do modelo
	glLoadIdentity();

	// Especifica posição do observador e do alvo
	//gluLookAt(0, 80, 200, 0, 0, 0, 0, 1, 0);
	gluLookAt(obsX, obsY, obsZ, 0, 0, 0, 0, 1, 0);
}

// Função callback chamada quando o tamanho da janela é alterado 
void AlteraTamanhoJanela(GLsizei w, GLsizei h)
{
	// Para previnir uma divisão por zero
	if (h == 0) h = 1;

	// Especifica o tamanho da viewport
	glViewport(0, 0, w, h);

	// Calcula a correção de aspecto
	fAspect = (GLfloat)w / (GLfloat)h;

	EspecificaParametrosVisualizacao();
}

// Função callback chamada para gerenciar eventos do mouse
void GerenciaMouse(int button, int state, int x, int y)
{
	if (button == GLUT_LEFT_BUTTON)
		if (state == GLUT_DOWN) {  // Zoom-in
			if (angle >= 10) angle -= 5;
		}
	if (button == GLUT_RIGHT_BUTTON)
		if (state == GLUT_DOWN) {  // Zoom-out
			if (angle <= 130) angle += 5;
		}
	EspecificaParametrosVisualizacao();
	glutPostRedisplay();
}

// Função callback chamada para gerenciar eventos de teclado
void GerenciaTeclado(unsigned char key, int x, int y)
{
	switch (key) {
	case 'A':
	case 'a':
		anima = true;
		break;
	case 'R':
	case 'r':
		reset = true;
		break;
	}
	glutPostRedisplay();
}

// Callback para gerenciar eventos do teclado para teclas especiais (F1, PgDn, entre outras)
void SpecialKeys(int key, int x, int y)
{
	switch (key) {
	case GLUT_KEY_LEFT:
		obsX -= 10;
		break;
	case GLUT_KEY_RIGHT:
		obsX += 10;
		break;
	case GLUT_KEY_UP:
		obsY += 10;
		break;
	case GLUT_KEY_DOWN:
		obsY -= 10;
		break;

	case GLUT_KEY_HOME:
		obsZ += 10;
		break;
	case GLUT_KEY_END:
		obsZ -= 10;
		break;
	}
	glLoadIdentity();
	gluLookAt(obsX, obsY, obsZ, 0, 0, 0, 0, 1, 0);
	glutPostRedisplay();
}

// Programa Principal
int main(void)
{
	glutInitDisplayMode(GLUT_DOUBLE | GLUT_RGB | GLUT_DEPTH);
	glutInitWindowSize(800, 700);
	glutCreateWindow("Visualizacao 3D");
	glutDisplayFunc(Desenha);
	glutReshapeFunc(AlteraTamanhoJanela);
	glutMouseFunc(GerenciaMouse);
	glutKeyboardFunc(GerenciaTeclado);
	glutSpecialFunc(SpecialKeys);
	glutTimerFunc(33, Timer, 1);
	Inicializa();
	glutMainLoop();
}