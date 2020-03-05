import json
stocksData = '{"hitBoard":"000233|002333","weakToStrong":"002444|300222"}'
stocksJson = json.loads(stocksData)
print(stocksJson['hitBoard'])
