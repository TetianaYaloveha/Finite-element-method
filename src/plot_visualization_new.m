function res = plot_visualization_new()
  pkg load geometry
  clear;
  clc;

  % ������� ��� � �����
  data = dlmread('solution_points.txt');

  % Extract x, y, and z columns
  x = data(:, 1);
  y = data(:, 2);
  z = data(:, 3);

  % �������� ������� �����
  n = size(x, 1);

  % ������������� ������� delaunay ��� ��������� ������� ����������
  tri = delaunay(x,y);

  % ��������� ���������� ������ � ������������� ������� trisurf
  trisurf(tri, x, y, z, 'EdgeColor', 'interp');

  % ������ ���� ���� �� ��������� �������
  xlabel('x');
  ylabel('y');
  zlabel('z');
  title('3D Plot with Triangles');


  hold on;
  scatter3(x, y, z, 20, 'r', 'filled');
end
