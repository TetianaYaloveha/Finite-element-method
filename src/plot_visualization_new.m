function res = plot_visualization_new()
  pkg load geometry
  clear;
  clc;

  % Зчитуємо дані з файлу
  data = dlmread('solution_points.txt');

  % Extract x, y, and z columns
  x = data(:, 1);
  y = data(:, 2);
  z = data(:, 3);

  % Отримуємо кількість точок
  n = size(x, 1);

  % Використовуємо функцію delaunay для отримання матриці трикутників
  tri = delaunay(x,y);

  % Створюємо тривимірний графік з використанням функції trisurf
  trisurf(tri, x, y, z, 'EdgeColor', 'interp');

  % Задаємо мітки осей та заголовок графіку
  xlabel('x');
  ylabel('y');
  zlabel('z');
  title('3D Plot with Triangles');


  hold on;
  scatter3(x, y, z, 20, 'r', 'filled');
end
