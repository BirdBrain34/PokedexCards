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

$sql = "SELECT DISTINCT cardRarity FROM cards";
$result = $conn->query($sql);

$rarities = array();

if ($result->num_rows > 0) {
    while ($row = $result->fetch_assoc()) {
        $rarities[] = $row['cardRarity'];
    }
}

$conn->close();

echo json_encode($rarities);
?>