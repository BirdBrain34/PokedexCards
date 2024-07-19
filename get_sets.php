<?php
header('Content-Type: application/json');

$servername = "localhost";
$username = "root";
$password = "";
$dbname = "pokedex";

$conn = new mysqli($servername, $username, $password, $dbname);

if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

$sql = "SELECT DISTINCT setName FROM `set`";
$result = $conn->query($sql);

$sets = array();

if ($result->num_rows > 0) {
    while ($row = $result->fetch_assoc()) {
        $sets[] = $row['setName'];
    }
}

$conn->close();

echo json_encode($sets);
?>
