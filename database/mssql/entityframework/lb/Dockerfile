FROM nginx:latest
COPY default.conf /etc/nginx/conf.d/
ENTRYPOINT /usr/sbin/nginx -g 'daemon off;' -c /etc/nginx/nginx.conf
