clear all, close all
%3D På stationära
M = csvread('fpsData.txt');
plot(M(:,1), M(:,2))
xlabel('Number of Agents')
ylabel('FPS')
title('FPS as a function of the number of agents')

%2D På stationära
N = csvread('fpsData2.txt');
hold on
plot(N(:,1), N(:,2))
legend('3D Agents', '2D Agents')