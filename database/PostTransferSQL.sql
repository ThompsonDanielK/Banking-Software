
INSERT INTO transfers
	(transfer_type_id, transfer_status_id, account_from, account_to, amount)
VALUES
	(1001, 2002, 4000, 4001, 100);

UPDATE 
	accounts
SET
	balance = balance - 100
WHERE
	user_id = 3000;

UPDATE 
	accounts
SET
	balance = balance + 100
WHERE
	user_id = 3001;


