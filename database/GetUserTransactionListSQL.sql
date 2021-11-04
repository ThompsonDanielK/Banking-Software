SELECT
t.transfer_id, t.transfer_type_id, t.account_from, t.account_to, t.amount, u.username
FROM
transfers t
INNER JOIN accounts a ON a.account_id = t.account_from OR a.account_id = t.account_to
INNER JOIN users u ON u.user_id = a.user_id
WHERE a.user_id = 3001