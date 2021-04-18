docker build -t bottle-cap-v2-image .

docker tag bottle-cap-v2-image registry.heroku.com/bottle-cap-v2/web


docker push registry.heroku.com/bottle-cap-v2/web

heroku container:release web -a bottle-cap-v2