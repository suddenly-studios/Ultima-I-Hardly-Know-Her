
build:
	docker buildx build --tag ultima-adventures --progress auto .

.PHONY: run
run:
	docker run -it -p 2593:2593 -v ./Backups:/opt/Ultima-Adventures/Backups -v ./Logs:/opt/Ultima-Adventures/Logs -v ./Saves:/opt/Ultima-Adventures/Saves ultima-adventures

.PHONY: stop
stop:
	docker stop ultima-adventures