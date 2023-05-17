function res = plot_visualization()
  pkg load geometry
  clear;
  clc;



  data = dlmread('solution_points.txt');

% Extract x, y, and z columns
  x = data(:, 1);
  y = data(:, 2);
  z = data(:, 3);

  % Plot the points using scatter3
  scatter3(x, y, z, 'filled');
  xlabel('x');
  ylabel('y');
  zlabel('z');
  title('3D Plot');


end;

