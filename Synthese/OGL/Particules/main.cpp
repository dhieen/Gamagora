#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include <glm/vec3.hpp>
#include <glm/mat4x4.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/ext/matrix_float4x4.hpp>
#include "stl.h"

#include <vector>
#include <iostream>
#include <random>
#include <sstream>
#include <fstream>
#include <string>
#include <math.h>

static void error_callback(int /*error*/, const char* description)
{
	std::cerr << "Error: " << description << std::endl;
}

static void key_callback(GLFWwindow* window, int key, int /*scancode*/, int action, int /*mods*/)
{
    if (key == GLFW_KEY_ESCAPE && action == GLFW_PRESS)
        glfwSetWindowShouldClose(window, GLFW_TRUE);
}

/* PARTICULES */
struct Particule {
	glm::vec3 position;
	glm::vec3 color;
	glm::vec3 speed;
};

std::vector<Particule> MakeParticules(const int n)
{
  std::default_random_engine generator;
  std::uniform_real_distribution<float> distribution01(0, 1);
  std::uniform_real_distribution<float> distributionWorld(-1, 1);

  std::vector<Particule> p;
  p.reserve(n);
  
  for(int i = 0; i < n; i++)
  {
	  p.push_back(Particule{
					 {
					  distributionWorld(generator),
					  distributionWorld(generator),
					  distributionWorld(generator)
					 },
					 {
					  distribution01(generator),
					  distribution01(generator),
					  distribution01(generator)
					 },
					 {
					  .1f * distribution01(generator),
					  .1f * distribution01(generator),
					  .1f * distribution01(generator)
					 },
		  });
  }

  return p;
}

GLuint MakeShader(GLuint t, std::string path)
{
	std::cout << path << std::endl;
	std::ifstream file(path.c_str(), std::ios::in);
    std::ostringstream contents;
    contents << file.rdbuf();
    file.close();

	const auto content = contents.str();
	std::cout << content << std::endl;
	
	const auto s = glCreateShader(t);

	GLint sizes[] = {(GLint) content.size()};
	const auto data = content.data();

	glShaderSource(s, 1, &data, sizes);
	glCompileShader(s);

	GLint success;
	glGetShaderiv(s, GL_COMPILE_STATUS, &success);
	if(!success)
	{
		GLchar infoLog[512];
		GLsizei l;
		glGetShaderInfoLog(s, 512, &l, infoLog);

		std::cout << infoLog << std::endl;
	}

	return s;
}

GLuint AttachAndLink(std::vector<GLuint> shaders)
{
	const auto prg = glCreateProgram();
	for(const auto s : shaders)
	{
		glAttachShader(prg, s);
	}

	glLinkProgram(prg);

	GLint success;
	glGetProgramiv(prg, GL_LINK_STATUS, &success);
	if(!success)
	{
		GLchar infoLog[512];
		GLsizei l;
		glGetProgramInfoLog(prg, 512, &l, infoLog);

		std::cout << infoLog << std::endl;
	}

	return prg;
}

int main(void)
{
    GLFWwindow* window;
    glfwSetErrorCallback(error_callback);

    if (!glfwInit())
        exit(EXIT_FAILURE);

    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 5);

    window = glfwCreateWindow(640, 480, "Simple example", NULL, NULL);

    if (!window)
    {
        glfwTerminate();
        exit(EXIT_FAILURE);
    }

    glfwSetKeyCallback(window, key_callback);
    glfwMakeContextCurrent(window);
    glfwSwapInterval(1);
    // NOTE: OpenGL error checks have been omitted for brevity

	if(!gladLoadGL()) {
		std::cerr << "Something went wrong!" << std::endl;
       exit(-1);
    }

	// Shader
	const auto vertex = MakeShader(GL_VERTEX_SHADER, "shader.vert");
	const auto fragment = MakeShader(GL_FRAGMENT_SHADER, "shader.frag");
	const auto program = AttachAndLink({vertex, fragment});
	auto index_pos = glGetAttribLocation(program, "position");
	auto index_col = glGetAttribLocation(program, "color");
	auto index_transform = glGetUniformLocation(program, "transform");

	glUseProgram(program);
	glPointSize(5.f);
	
	// PARTICLES
	// Data
	const size_t nParticules = 1000;
	auto particules = MakeParticules(nParticules);

	// Buffers
	GLuint p_vbo, p_vao;
	glGenBuffers(1, &p_vbo);
	glGenVertexArrays(1, &p_vao);

	// Bindings
	glBindVertexArray(p_vao);
	glBindBuffer(GL_ARRAY_BUFFER, p_vbo);
	glBufferData(GL_ARRAY_BUFFER, nParticules * sizeof(Particule), particules.data(), GL_STATIC_DRAW);

	glVertexAttribPointer(index_pos, 3, GL_FLOAT, GL_FALSE, sizeof(Particule), nullptr);
	glEnableVertexAttribArray(index_pos);
	glVertexAttribPointer(index_col, 3, GL_FLOAT, GL_FALSE, sizeof(Particule), (void*)(3 * sizeof(float)));
	glEnableVertexAttribArray(index_col);	


	// TRIANGLES
	// Data
	std::vector<Triangle> triangles = ReadStl("logo.stl");
	size_t nTriangles = triangles.size();

	// Buffers
	GLuint t_vbo, t_vao;
	glGenBuffers(1, &t_vbo);
	glGenVertexArrays(1, &t_vao);

	// Bindings
	glBindVertexArray(t_vao);
	glBindBuffer(GL_ARRAY_BUFFER, t_vbo);
	glBufferData(GL_ARRAY_BUFFER, nTriangles * sizeof(Triangle), triangles.data(), GL_STATIC_DRAW);

	glVertexAttribPointer(index_pos, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), nullptr);
	glEnableVertexAttribArray(index_pos);
	
	glm::mat4x4 logoTransform = glm::mat4(1.f);
	glm::mat4x4 particlesTransform = glm::mat4(1.f);
	float logoScale = 0.04f;
	float logoRotation = .0f;
	float cameraOrbit = 0.0f;
	float time = 0.0f;

    while (!glfwWindowShouldClose(window))
    {
		time++;

		// Update particles positions
		for (int i = 0; i < nParticules; i++)
		{
			if (particules[i].position.y > -1.5f)
				particules[i].position.y -= particules[i].speed.y;
			else
				particules[i].position.y = 1.5f;
		}

		// Update logo transform
		logoScale = 0.01f + cos((time)/10.0f) * 0.001f;
		logoRotation = time/20.0;
		logoTransform = glm::mat4(1.f);
		//logoTransform = glm::rotate(logoTransform, logoRotation, glm::vec3(0.f, 0.f, 1.f));
		logoTransform = glm::scale(logoTransform, glm::vec3(logoScale, logoScale, logoScale));
		logoTransform = logoTransform * glm::lookAt(glm::vec3(10.0f * cos(time / 80.0f), 0.0f, 10.0f * sin(time / 80.0f)), glm::vec3(), glm::vec3(0.0f, 1.0f, 0.0f));

        int width, height;
        glfwGetFramebufferSize(window, &width, &height);

        glViewport(0, 0, width, height);

        glClear(GL_COLOR_BUFFER_BIT);
		glClearColor(1.0f, 1.0f, 1.0f, 1.0f);

		// Set triangle buffer and uniform
		glBindVertexArray(t_vao);
		glUniformMatrix4fv(index_transform, 1, GL_FALSE, &logoTransform[0][0]);
		// Draw triangles
		glDrawArrays(GL_TRIANGLES, 0, 3 * nTriangles);

		// Set Particle buffer and uniform
		glBindVertexArray(p_vao);
		glBindBuffer(GL_ARRAY_BUFFER, p_vbo);
		glUniformMatrix4fv(index_transform, 1, GL_FALSE, &particlesTransform[0][0]);
		// Update buffer
		glBufferSubData(GL_ARRAY_BUFFER, 0, nParticules * sizeof(Particule), particules.data());
		// Draw particles
		glDrawArrays(GL_POINTS, 0, nParticules);

        glfwSwapBuffers(window);
        glfwPollEvents();
    }
    glfwDestroyWindow(window);
    glfwTerminate();
    exit(EXIT_SUCCESS);
}
