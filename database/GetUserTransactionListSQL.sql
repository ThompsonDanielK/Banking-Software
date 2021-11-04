SELECT
	t.transfer_id, t.transfer_type_id, t.account_from, t.account_to, t.amount, u.username, uto.username
FROM
	transfers t
	INNER JOIN accounts a ON a.account_id = t.account_from 
	INNER JOIN users u ON u.user_id = a.user_id
	INNER JOIN accounts ato ON ato.account_id = t.account_to
	INNER JOIN users uto ON uto.user_id = ato.user_id
WHERE 
	a.user_id = 3000
	OR
	ato.user_id = 3000

