function obj=fitness(X)
%% ���Ż���Ŀ�꺯��
% X��ÿ��Ϊһ������
col=size(X,1);
for i=1:col
    %obj(i,1)=21.5+X(i,1)*sin(4*pi*X(i,1))+X(i,2)*sin(20*pi*X(i,2));
    obj(i,1)=((sin(sqrt(X(i,1).^2+X(i,2).^2))).^2-0.5)./((1+0.001.*(X(i,1).^2+X(i,2).^2)).^2)-0.5;  %һ��Ҫ�õ��./
end
